using Fido2NetLib.Objects;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;


namespace Fido2Apis.Domain.Fido2Auth
{
    using Fido2Apis.Domain.User;
    public class Fido2Credential
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("user_credential_id")]
        public byte[] UserId { get; set; }

        [Column("public_key")]
        public byte[] PublicKey { get; set; }

        [Column("user_handle")]
        public byte[] UserHandle { get; set; }

        [Column("signature_counter")]
        public uint SignatureCounter { get; set; }

        [Column("cred_type")]
        public string CredType { get; set; }

        [Column("reg_date")]
        public DateTime RegDate { get; set; }

        [Column("aaguid")]
        public Guid AaGuid { get; set; }

        [NotMapped]
        public PublicKeyCredentialDescriptor Descriptor
        {
            get => string.IsNullOrWhiteSpace(DescriptorJson) ? null : JsonSerializer.Deserialize<PublicKeyCredentialDescriptor>(DescriptorJson);
            set => DescriptorJson = JsonSerializer.Serialize(value);
        }

        [Column("descriptor_json")]
        public string DescriptorJson { get; set; }

/*        [Column("user_password_id")]
        public Guid UserPasswordId { get; set; }

        public User User { get; set; } = null;*/
    }
}
