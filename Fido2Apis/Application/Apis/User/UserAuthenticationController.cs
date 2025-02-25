using Fido2Apis.Domain.User;
using Microsoft.AspNetCore.Mvc;

namespace Fido2Apis.Application.Apis.User
{
    [Route("v1/api/user/")]
    [ApiController]
    public class UserAuthenticationController : BaseController
    {
        public ILogger<UserAuthenticationController> _logger;
        public IUserAuthenticationService _userAuthenticationService;

        public UserAuthenticationController(ILogger<UserAuthenticationController> logger, IUserAuthenticationService userAuthenticationService)
        {
            _logger = logger;
            _userAuthenticationService = userAuthenticationService;
        }

        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate(UserAuthenticationRequest userAuthenticationRequest)
        {
            _logger.LogInformation("User authentication");

            return ProcessApiResponse(_userAuthenticationService.Authenticate(userAuthenticationRequest));
        }
    }
}
