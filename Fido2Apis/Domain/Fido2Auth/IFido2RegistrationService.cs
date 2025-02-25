using Fido2Apis.Application.Apis.Fido2Auth;
using Fido2Apis.Application.Response;
using Fido2NetLib;

namespace Fido2Apis.Domain.Fido2Auth
{
    public interface IFido2RegistrationService
    {
        public ResponseObject CreateCredentialsOptions(Fido2RegistrationRequest fido2RegistrationRequest);
        public Task<ResponseObject> CreateCredentials(AuthenticatorAttestationRawResponse authenticatorAttestationRawResponse);
    }
}
