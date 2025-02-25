using Fido2Apis.Domain.Fido2Auth;
using Fido2NetLib;
using Microsoft.AspNetCore.Mvc;

namespace Fido2Apis.Application.Apis.Fido2Auth
{
    [Route("v1/api/fido2/authentication/")]
    [ApiController]
    public class Fido2AuthenticationController : BaseController
    {
        private readonly ILogger<Fido2AuthenticationController> _logger;
        private readonly IFido2AuthenticationService _fido2AuthenticationService;

        public Fido2AuthenticationController(ILogger<Fido2AuthenticationController> logger, IFido2AuthenticationService fido2AuthenticationService)
        {
            _logger = logger;
            _fido2AuthenticationService = fido2AuthenticationService;
        }

        [HttpPost]
        [Route("initiate")]
        public IActionResult AuthenticationInitiate([FromBody] Fido2AuthenticationRequest fido2AuthenticationRequest)
        {
            _logger.LogInformation("Authentication initiate");

            return ProcessApiResponse(_fido2AuthenticationService.GetAssertionOptions(fido2AuthenticationRequest));
        }

        [HttpPost]
        [Route("finish")]
        public async Task<IActionResult> RegistrationFinish([FromBody] AuthenticatorAssertionRawResponse authenticatorAssertionRawResponse)
        {
            _logger.LogInformation("Authentication finish");

            return ProcessApiResponse(await _fido2AuthenticationService.MakeAssertion(authenticatorAssertionRawResponse));
        }
    }
}
