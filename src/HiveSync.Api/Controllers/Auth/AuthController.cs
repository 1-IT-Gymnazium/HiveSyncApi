using HiveSync.Api.Controllers.Auth.Models;
using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Entities.Identity;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Services;
using HiveSync.Data;
using HiveSync.Utilities;
using HiveSync.Utilities.Error;
using HiveSync.Utilities.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HiveSync.Api.Controllers.Auth;

/// <summary>
/// API controller responsible for authentication and authorization operations.
/// Handles user registration, login, token management, logout, and email confirmation.
/// </summary>
/// <param name="clock">Provides the current time using NodaTime.</param>
/// <param name="dbContext">Database context used for persistence.</param>
/// <param name="userManager">ASP.NET Core Identity user manager.</param>
/// <param name="signInManager">ASP.NET Core Identity sign-in manager.</param>
/// <param name="emailSenderService">Service responsible for sending emails.</param>
/// <param name="jwtSettings">JWT configuration settings.</param>
/// <param name="environmentOptions">Environment-specific configuration values.</param>
[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IClock clock,
    AppDbContext dbContext,
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    EmailSenderService emailSenderService,
    IOptions<JwtSettings> jwtSettings,
    IOptions<EnvironmentSettings> environmentOptions)
    : ControllerBase
{
    private readonly IClock _clock = clock;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly EnvironmentSettings _environmentOptions = environmentOptions.Value;
    private readonly EmailSenderService _emailSenderService = emailSenderService;

    /// <summary>
    /// Registers a new user account and sends an email confirmation link.
    /// Also creates a default inbox project for the user.
    /// </summary>
    /// <param name="model">User registration data.</param>
    /// <returns>No content when registration succeeds.</returns>
    /// <response code="204">User successfully registered.</response>
    /// <response code="400">Validation error.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("Register")]
    public async Task<ActionResult> Register([FromBody] RegisterModel model)
    {
        var errorBuilder = new ValidationErrorBuilder();

        foreach (var key in ModelState.Keys)
        {
            var errors = ModelState[key]?.Errors;
            if (errors == null) continue;
            foreach (var error in errors)
                errorBuilder.Add(key, error.ErrorMessage);
        }

        var validator = new PasswordValidator<AppUser>();
        var now = _clock.GetCurrentInstant();

        var newUser = new AppUser
        {
            Id = Guid.NewGuid(),
            DisplayName = model.DisplayName,
            Email = model.Email,
            UserName = model.Email
        }.SetCreateBySystem(now);

        var checkPassword = await validator.ValidateAsync(_userManager, newUser, model.Password);
        if (!checkPassword.Succeeded)
        {
            foreach (var error in checkPassword.Errors)
                errorBuilder.Add(nameof(model.Password), error.Description);

            throw new ApiValidationException(errorBuilder.Build());
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var result = await _userManager.CreateAsync(newUser);
            if (!result.Succeeded)
                throw new ConflictException(ErrorMessages.UserAlreadyExists());

            await _userManager.AddPasswordAsync(newUser, model.Password);

            var inbox = new Project
            {
                Id = Guid.NewGuid(),
                Name = "Inbox",
                OwnerId = newUser.Id,
                Color = "DEFAULT",
                IsDefault = true
            }.SetCreateBySystem(now);

            _dbContext.Add(inbox);

            newUser.InboxId = inbox.Id;
            await _userManager.UpdateAsync(newUser);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            // Send email using shared method
            await SendConfirmationEmailAsync(newUser);

            return NoContent();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Authenticates a user and issues access and refresh tokens.
    /// Tokens are stored in HTTP-only cookies.
    /// </summary>
    /// <param name="model">User login credentials.</param>
    /// <returns>No content when authentication succeeds.</returns>
    /// <response code="204">Login successful.</response>
    /// <response code="401">Invalid email or password.</response>
    /// <response code="403">Account is locked.</response>x
    [HttpPost("Login")]
    public async Task<ActionResult> Login(
        [FromBody] LoginModel model)
    {
        var normalizedEmail = model.Email.ToUpperInvariant();
        var user = await _userManager.Users
            .SingleOrDefaultAsync(x => x.EmailConfirmed && x.NormalizedEmail == normalizedEmail)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidEmailOrPassword());
        var signInResult = await _signInManager
            .CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

        if (signInResult.IsLockedOut)
            throw new ForbiddenException(ErrorMessages.AccountLockedOut());

        if (!signInResult.Succeeded)
            throw new UnauthorizedException(ErrorMessages.InvalidEmailOrPassword());

        var accessToken = GenerateAccessToken(user, _jwtSettings.AccessTokenExpirationInMinutes);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id, _jwtSettings.RefreshTokenExpirationInDays);

        var accessOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // true in production - for https
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes)
        };

        var refreshOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // true in production - for https
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays)
        };

        Response.Cookies.Append("AccessToken", accessToken, accessOptions);
        Response.Cookies.Append("RefreshToken", refreshToken, refreshOptions);

        return NoContent();
    }

    /// <summary>
    /// Confirms a user's email using a confirmation token.
    /// </summary>
    /// <param name="model">Email and confirmation token.</param>
    /// <returns>No content if confirmation succeeds.</returns>
    /// <response code="204">Email successfully confirmed.</response>
    /// <response code="401">Invalid email.</response>
    /// <response code="409">Email already confirmed.</response>
    /// <response code="400">Invalid confirmation token.</response>
    [HttpPost("ValidateToken")]
    public async Task<ActionResult> ValidateToken([FromBody] TokenModel model)
    {
        var normalizedMail = model.Email.ToUpperInvariant();
        var user = await _userManager.Users
            .SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedMail)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidEmail());

        if (user.EmailConfirmed)
            throw new ConflictException(ErrorMessages.AlreadyConfirmed());

        var unescapedToken = Uri.UnescapeDataString(model.Token);
        var result = await _userManager.ConfirmEmailAsync(user, unescapedToken);

        if (!result.Succeeded)
        {
            // If the email is now confirmed, treat as success (race condition)
            user = await _userManager.Users
                .SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedMail);

            if (user?.EmailConfirmed ?? false)
                return NoContent();

            // Otherwise, token really failed
            var tokenError = result.Errors.FirstOrDefault()?.Code.ToLowerInvariant() ?? "";
            var errorBuilder = new ValidationErrorBuilder();

            if (tokenError.Contains("invalid") || tokenError.Contains("expired"))
                errorBuilder.Add(nameof(model.Token), "Confirmation token is invalid or expired.");
            else
                errorBuilder.Add(nameof(model.Token), "Failed to confirm email.");

            throw new ApiValidationException(errorBuilder.Build());
        }

        return NoContent();
    }

    /// <summary>
    /// Resend email confirmation link to a user.
    /// </summary>
    /// <param name="model">Email of the user to resend confirmation for.</param>
    /// <returns>No content if email sent successfully.</returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("ResendConfirmation")]
    public async Task<ActionResult> ResendConfirmation([FromBody] ResendConfirmationEmailModel model)
    {
        var normalizedEmail = model.Email.ToUpperInvariant();
        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail)
                   ?? throw new NotFoundException(ErrorMessages.NotFound(nameof(User)));

        if (user.EmailConfirmed)
            return NoContent(); // silently succeed if already confirmed

        await SendConfirmationEmailAsync(user);
        return NoContent();
    }

    /// <summary>
    /// Returns information about the currently authenticated user.
    /// </summary>
    /// <returns>Basic information about the logged-in user.</returns>
    /// <response code="200">Returns the logged user information.</response>
    /// <response code="401">User is not authenticated.</response>
    [AllowAnonymous]
    [HttpGet("UserInfo")]
    public async Task<ActionResult<LoggedUserModel>> GetUserInfo()
    {
        if (!User.Identities.Any(x => x.IsAuthenticated))
            throw new UnauthorizedException(ErrorMessages.Unauthorized());

        var id = User.GetUserId();
        var user = await _userManager.Users
            .Where(x => x.Id == id)
            .AsNoTracking()
            .SingleAsync();

        var loggedModel = new LoggedUserModel
        {
            Id = user.Id,
            Name = user.DisplayName,
            InboxId = user.InboxId.ToString(),
        };

        return loggedModel;
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token stored in cookies.
    /// Generates new access and refresh tokens.
    /// </summary>
    /// <returns>No content if refresh succeeds.</returns>
    /// <response code="204">Token refreshed successfully.</response>
    /// <response code="401">Refresh token invalid, expired, or missing.</response>
    [HttpPost("Refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("RefreshToken", out var incomingToken))
            throw new UnauthorizedException(ErrorMessages.RefreshTokenNotFound());

        var hashedToken = Hash(incomingToken);

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == hashedToken);

        if (storedToken == null || storedToken.ExpiresAt < _clock.GetCurrentInstant() || storedToken.RevokedAt != null)
            throw new UnauthorizedException(ErrorMessages.RefreshTokenInvalidOrExpired());

        // Generate new access and refresh tokens
        var user = await _dbContext.Users.FindAsync(storedToken.UserId)
            ?? throw new NotFoundException(ErrorMessages.UserNotFound());

        // Generate new tokens
        var newAccessToken = GenerateAccessToken(user, _jwtSettings.AccessTokenExpirationInMinutes);
        var newRefreshToken = await GenerateRefreshTokenAsync(user.Id, _jwtSettings.RefreshTokenExpirationInDays);

        storedToken.RevokedAt = _clock.GetCurrentInstant();
        await _dbContext.SaveChangesAsync();

        Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // For HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays)
        });

        Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // v dev může být false
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes)
        });

        return NoContent();
    }

    /// <summary>
    /// Logs the current user out by revoking the refresh token
    /// and removing authentication cookies.
    /// </summary>
    /// <returns>No content.</returns>
    /// <response code="204">Logout successful.</response>
    [Authorize]
    [HttpPost("Logout")]
    public async Task<ActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("RefreshToken", out var incomingToken))
            return NoContent();

        var hashedToken = Hash(incomingToken);

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == hashedToken);

        if (storedToken == null
            || storedToken.ExpiresAt < _clock.GetCurrentInstant()
            || storedToken.RevokedAt != null)
        {
            return NoContent();
        }

        storedToken.ExpiresAt = _clock.GetCurrentInstant();
        await _dbContext.SaveChangesAsync();

        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");
        return NoContent();
    }

    /// <summary>
    /// Sends password reset email if user exists.
    /// </summary>
    /// <param name="model">User email.</param>
    /// <returns>No content.</returns>
    [HttpPost("ForgotPassword")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        var normalizedEmail = model.Email.ToUpperInvariant();

        var user = await _userManager.Users
            .SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail);

        if (user == null)
            throw new InternalServerException("There was an unexpected error. Please try again later.");

        if (!user!.EmailConfirmed)
            throw new ForbiddenException("Account must be confirmed to reset password.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var escapedToken = Uri.EscapeDataString(token);

        var url = $"{_environmentOptions.FrontendHostUrl}{_environmentOptions.FrontendResetPasswordUrl}?token={escapedToken}&email={user.Email}";

        var logoUrl = "https://raw.githubusercontent.com/1-IT-Gymnazium/HiveSyncUi/1ef5eb91efa59425e19edff0bce5b70da1f69c4b/app/frontend/src/assets/logo/logo-icon.svg";

        var emailHtml = $"""
<div style="background-color:#282828; color:#ffffff; font-family:sans-serif; padding:20px;">
    <div style="text-align:center; margin-bottom:20px;">
        <img src="{logoUrl}" alt="HiveSync Logo" width="120" style="display:block; margin:0 auto 10px;" />
        <h1 style="color:#cc8613;">HiveSync</h1>
    </div>
    <div style="background-color:#303030; padding:20px; border-radius:8px;">
        <h2 style="color:#cc8613;">Reset Your Password</h2>
        <p>To reset your password, click the button below. The link is valid for 24 hours.</p>
        <a href="{url}" style="display:inline-block; background-color:#cc8613; color:#fff; padding:10px 20px; border-radius:4px; text-decoration:none;">Reset Password</a>
    </div>
</div>
""";

        await _emailSenderService.AddEmail(
            "HiveSync Password Reset",
            emailHtml,
            user.Email!);

        return NoContent();
    }

    /// <summary>
    /// Resets user password using reset token.
    /// </summary>
    /// <param name="model">Reset password data.</param>
    /// <returns>No content.</returns>
    [HttpPost("ResetPassword")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        var normalizedEmail = model.Email.ToUpperInvariant();

        var user = await _userManager.Users
            .SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail)
            ?? throw new UnauthorizedException(ErrorMessages.InvalidEmail());

        var validator = new PasswordValidator<AppUser>();

        var checkPassword = await validator.ValidateAsync(_userManager, user, model.NewPassword);

        if (!checkPassword.Succeeded)
        {
            var errorBuilder = new ValidationErrorBuilder();

            foreach (var error in checkPassword.Errors)
                errorBuilder.Add(nameof(model.NewPassword), error.Description);

            throw new ApiValidationException(errorBuilder.Build());
        }

        var passwordCheck = await _userManager.CheckPasswordAsync(user, model.NewPassword);
        if (passwordCheck)
            throw new ApiValidationException(
                new ValidationErrorBuilder().Add(nameof(model.NewPassword), "New password cannot be the same as the previous password.").Build()
            );

        var unescapedToken = Uri.UnescapeDataString(model.Token);

        var result = await _userManager.ResetPasswordAsync(
            user,
            unescapedToken,
            model.NewPassword);

        if (!result.Succeeded)
        {
            var errorBuilder = new ValidationErrorBuilder();

            foreach (var error in result.Errors)
                errorBuilder.Add(nameof(model.Token), error.Description);

            throw new ApiValidationException(errorBuilder.Build());
        }

        return NoContent();
    }

    /// <summary>
    /// Test endpoint requiring authentication.
    /// Used to verify authorization configuration.
    /// </summary>
    /// <returns>Simple success message.</returns>
    /// <response code="200">Endpoint successfully reached.</response>
    [Authorize]
    [HttpGet("TestAuth")]
    public ActionResult TestAuth()
        => Ok("Succesfully reached endpoint!");

    /// <summary>
    /// Endpoint for wakening backend services.
    /// </summary>
    /// <returns> Returns a simple "Pong!" response.</returns>
    /// <response code="200">Returns "Pong!" to indicate the service is awake.</response>
    [HttpGet("Ping")]
    public ActionResult Ping()
        => Ok("Pong!");

    /// <summary>
    /// Generates and stores a refresh token for a user.
    /// </summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="expirationInDays">Refresh token expiration in days.</param>
    /// <returns>The generated refresh token.</returns>
    private async Task<string> GenerateRefreshTokenAsync(Guid userId, int expirationInDays)
    {
        var refreshToken = Guid.NewGuid().ToString();
        var data = Request.Headers.UserAgent.ToString();

        var now = _clock.GetCurrentInstant();
        _dbContext.Add(
            new RefreshToken
            {
                UserId = userId,
                Token = Hash(refreshToken),
                CreatedAt = now,
                ExpiresAt = now.Plus(Duration.FromDays(expirationInDays)),
                RequestInfo = data,
            });
        await _dbContext.SaveChangesAsync();
        return refreshToken;
    }

    /// <summary>
    /// Generates a signed JWT access token for a user.
    /// </summary>
    /// <param name="user">Authenticated user.</param>
    /// <param name="expirationInMinutes">Token expiration time in minutes.</param>
    /// <returns>Serialized JWT token string.</returns>
    private string GenerateAccessToken(
        AppUser user,
        int expirationInMinutes)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email!),
            new (JwtRegisteredClaimNames.Name, user.UserName!),
            new ("inbox_id", user.InboxId.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Sends email confirmation link to a user.
    /// Used both on registration and on resend request.
    /// </summary>
    /// <param name="user">The user to send email to.</param>
    private async Task SendConfirmationEmailAsync(AppUser user)
    {
        // Reset security stamp to invalidate old tokens
        await _userManager.UpdateSecurityStampAsync(user);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var escapedToken = Uri.EscapeDataString(token);

        var url =
            $"{_environmentOptions.FrontendHostUrl}{_environmentOptions.FrontendConfirmUrl}?token={escapedToken}&email={user.Email}";

        var logoUrl = "https://raw.githubusercontent.com/1-IT-Gymnazium/HiveSyncUi/1ef5eb91efa59425e19edff0bce5b70da1f69c4b/app/frontend/src/assets/logo/logo-icon.svg";

        var emailHtml = $"""
<div style="background-color:#282828; color:#ffffff; font-family:sans-serif; padding:20px;">
    <div style="text-align:center; margin-bottom:20px;">
        <img src="{logoUrl}" alt="HiveSync Logo" width="120" style="display:block; margin:0 auto 10px;" />
        <h1 style="color:#cc8613;">HiveSync</h1>
    </div>
    <div style="background-color:#303030; padding:20px; border-radius:8px;">
        <h2 style="color:#cc8613;">Confirm your email</h2>
        <p>Click the button below to confirm your email. The link is valid for 24 hours.</p>
        <a href="{url}" style="display:inline-block; background-color:#cc8613; color:#fff; padding:10px 20px; border-radius:4px; text-decoration:none;">Confirm Email</a>
    </div>
</div>
""";

        await _emailSenderService.AddEmail(
            "HiveSync Email Confirmation",
            emailHtml,
            user.Email!);
    }

    /// <summary>
    /// Computes a SHA256 hash of the provided token.
    /// Used to store refresh tokens securely.
    /// </summary>
    /// <param name="token">Raw token value.</param>
    /// <returns>Base64 encoded hash.</returns>
    public static string Hash(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Test endpoint that sends a test email using the configured email service.
    /// </summary>
    /// <param name="service">Email sending service.</param>
    /// <returns>No content.</returns>
    [HttpGet("TestMail")]
    public async Task<ActionResult> Test(
    [FromServices] EmailSenderService service)
    {
        await _emailSenderService.AddEmail("Very Interesting Subject Message Or Something", "Aaaaaaaaaaa, no cat images for you!! >:(", "test@test.cz");
        return NoContent();
    }
}
