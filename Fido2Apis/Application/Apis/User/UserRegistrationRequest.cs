using Fido2Apis.Infra.CustomModelAttribute;
using System.ComponentModel.DataAnnotations;

namespace Fido2Apis.Application.Apis.User
{
    public class UserRegistrationRequest
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Username cannot be more than 255 characters")]
        public string Username { get; set; }

        [Required]
        [ValidatePassword]
        [MaxLength(255, ErrorMessage = "Username cannot be more than 255 characters")]
        public string Password { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Username cannot be more than 255 characters")]
        public string ConfirmPassword { get; set; }

        public override string? ToString()
        {
            return "Username: " + Username;
        }
    }
}
