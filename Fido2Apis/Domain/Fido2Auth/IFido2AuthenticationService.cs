using Fido2Apis.Application.Apis.Fido2Auth;
using Fido2Apis.Application.Response;
using Fido2NetLib;

namespace Fido2Apis.Domain.Fido2Auth
{
    public interface IFido2AuthenticationService
    {
        public ResponseObject GetAssertionOptions(Fido2AuthenticationRequest fido2AuthenticationRequest);
        public Task<ResponseObject> MakeAssertion(AuthenticatorAssertionRawResponse authenticatorAssertionRawResponse);
    }
}
