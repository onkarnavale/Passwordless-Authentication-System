using Fido2Apis.Domain.User;
using Microsoft.AspNetCore.Mvc;

namespace Fido2Apis.Application.Apis.User
{
    [Route("v1/api/user/")]
    [ApiController]
    public class UserRegistrationController : BaseController
    {
        public ILogger<UserRegistrationController> _logger;
        public IUserRegistrationService _userRegistrationService;

        public UserRegistrationController(ILogger<UserRegistrationController> logger, IUserRegistrationService userRegistrationService)
        {
            _logger = logger;
            _userRegistrationService = userRegistrationService;
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserRegistrationRequest userRegistrationRequest)
        {
            _logger.LogInformation("User registration");

            return ProcessApiResponse(_userRegistrationService.Register(userRegistrationRequest));
        }
    }
}
