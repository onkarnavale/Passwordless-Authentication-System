using System.ComponentModel.DataAnnotations;

namespace Fido2Apis.Application.Apis.Fido2Auth
{
    public class Fido2RegistrationRequest
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Name cannot be more than 255 characters")]
        public string Username { get; set; }

        public override string? ToString()
        {
            return "Name: " + Username;
        }
    }
}
