using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HiveSync.Api.Controllers.Auth.Models
{
    /// <summary>
    /// Represents the logged-in user information returned to the frontend.
    /// </summary>
    public class LoggedUserModel
    {
        /// <summary>
        /// The unique identifier of the user.
        /// </summary>
        [property: JsonProperty("id")]
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Display name of the user.
        /// </summary>
        [property: JsonProperty("name")]
        public string? Name { get; set; } = string.Empty;

        /// <summary>
        /// User's Inbox project ID.
        /// </summary>
        [property: JsonProperty("inboxId")]
        [Required]
        public string InboxId { get; set; } = null!;
    }
}
