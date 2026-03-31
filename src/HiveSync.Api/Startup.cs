using HiveSync.Application;
using HiveSync.Application.BackgroundServices;
using HiveSync.Application.Contracts.Entities.Identity;
using HiveSync.Application.Contracts.Interfaces;
using HiveSync.Application.Services;
using HiveSync.Data;
using HiveSync.Utilities;
using HiveSync.Utilities.Error;
using HiveSync.Utilities.Interfaces;
using HiveSync.Utilities.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using NodaTime;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;

namespace HiveSync.Api;

/// <summary>
/// Application startup configuration responsible for registering services
/// and configuring the HTTP request pipeline.
/// </summary>
/// <param name="configuration">Application configuration source.</param>
/// <param name="hostEnvironment">Provides information about the hosting environment.</param>
public class Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
{
    /// <summary>
    /// Name of the application used in documentation and metadata.
    /// </summary>
    public const string APPLICATION_NAME = "HiveSync";

    /// <summary>
    /// Name of the application used in documentation and metadata.
    /// </summary>
    private const string CorsPolicy = "_corsPolicy";

    /// <summary>
    /// Registers application services into the dependency injection container.
    /// </summary>
    /// <param name="services">Service collection used for dependency injection.</param>
    public void ConfigureServices(IServiceCollection services)
    {

        //DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new NoNullAllowedException();

        services.AddDataLayer(connectionString);

        //CORS
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy, policy =>
            {
                policy
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // Identity
        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
        {
            // Password policy
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // SignIn options
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Configure token lifespan for email confirmation and password reset
        services.Configure<DataProtectionTokenProviderOptions>(opts =>
        {
            opts.TokenLifespan = TimeSpan.FromHours(24);
        });

        // Authentication with JWT
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        var jwtSettings = configuration.GetRequiredSection(nameof(JwtSettings)).Get<JwtSettings>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings!.SecretKey)),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.TryGetValue("AccessToken", out var token))
                        context.Token = token;
                    return Task.CompletedTask;
                }
            };
        });

        // Settings
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.Configure<EnvironmentSettings>(configuration.GetSection("EnvironmentSettings"));

        // MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(MediatrHandlerResolver).Assembly);
        });

        // Core services
        services.AddSingleton<IClock>(SystemClock.Instance);
        services.AddScoped<IApplicationMapper, ApplicationMapper>();

        // Email services
        services.AddScoped<EmailSenderService>();
        services.AddHostedService<EmailSenderBackgroundService>();

        // User context
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        // Controllers
        services.AddEndpointsApiExplorer();
        services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            // Disables automatic 400 responses for model validation
            options.SuppressModelStateInvalidFilter = true;
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        });

        //Swagger
        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo { Title = $"{APPLICATION_NAME} API ", Version = "v1" });

            // Configure JWT Authentication in Swagger
            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token without the 'Bearer' prefix.\n\nExample: abc123xyz"
            });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            config.SupportNonNullableReferenceTypes();
        });
    }

    /// <summary>
    /// Configures the HTTP request processing pipeline.
    /// </summary>
    /// <param name="app">Application builder used to configure middleware.</param>
    public void Configure(IApplicationBuilder app)
    {
        if (hostEnvironment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{APPLICATION_NAME} API v1");
            });
        }

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseRouting();

        if (hostEnvironment.IsDevelopment())
        {
            app.UseCors(CorsPolicy);
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
