using Fido2Apis.Application.Apis.User;
using Fido2Apis.Application.Response;

namespace Fido2Apis.Domain.User
{
    public interface IUserAuthenticationService
    {
        public ResponseObject Authenticate(UserAuthenticationRequest userAuthenticationRequest);
    }
}
