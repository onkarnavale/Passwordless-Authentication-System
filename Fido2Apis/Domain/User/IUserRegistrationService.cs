using Fido2Apis.Application.Apis.User;
using Fido2Apis.Application.Response;

namespace Fido2Apis.Domain.User
{
    public interface IUserRegistrationService
    {
        public ResponseObject Register(UserRegistrationRequest userRegistrationRequest);
    }
}
