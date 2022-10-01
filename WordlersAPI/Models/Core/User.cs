using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WordlersAPI.Models.Core
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public long Points { get; set; }

        public DateTimeOffset LastLoggedInAt { get; set; }
        public DateTimeOffset LastPasswordChangedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
