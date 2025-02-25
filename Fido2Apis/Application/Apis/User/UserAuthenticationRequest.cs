using System.ComponentModel.DataAnnotations;

namespace Fido2Apis.Application.Apis.User
{
    public class UserAuthenticationRequest
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Username cannot be more than 255 characters")]
        public string Username { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Username cannot be more than 255 characters")]
        public string Password { get; set; }

        public override string? ToString()
        {
            return "Username: " + Username;
        }
    }
}
