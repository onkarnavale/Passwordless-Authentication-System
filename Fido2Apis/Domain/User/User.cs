using Fido2Apis.Domain.Fido2Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fido2Apis.Domain.User
{
    public class User
    {
        [Key]
        [Column("id")]
        public Guid id {  get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        public ICollection<Fido2Credential> Fido2Credentials { get; set; } = new List<Fido2Credential>();

        public override string? ToString()
        {
            return "Id: " + id + "; Username: " + Username + "; Password: " + Password;  
        }
    }
}
