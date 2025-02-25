using Fido2Apis.Domain.Fido2Auth;
using Fido2NetLib;
using Microsoft.AspNetCore.Mvc;

namespace Fido2Apis.Application.Apis.Fido2Auth
{
    [Route("v1/api/fido2/registration/")]
    [ApiController]
    public class Fido2RegistrationController : BaseController
    {
        private readonly ILogger<Fido2RegistrationController> _logger;
        private readonly IFido2RegistrationService _fido2RegistrationService;

        public Fido2RegistrationController(ILogger<Fido2RegistrationController> logger, IFido2RegistrationService fido2RegistrationService)
        {
            _logger = logger;
            _fido2RegistrationService = fido2RegistrationService;
        }

        [HttpPost]
        [Route("initiate")]
        public IActionResult RegistrationInitiate([FromBody] Fido2RegistrationRequest fido2RegistrationRequest)
        {
            _logger.LogInformation("Registration initiate");

            return ProcessApiResponse(_fido2RegistrationService.CreateCredentialsOptions(fido2RegistrationRequest));
        }

        [HttpPost]
        [Route("finish")]
        public async Task<IActionResult> RegistrationFinish([FromBody] AuthenticatorAttestationRawResponse authenticatorAttestationRawResponse)
        {
            _logger.LogInformation("Registration finish");

            return ProcessApiResponse(await _fido2RegistrationService.CreateCredentials(authenticatorAttestationRawResponse));
        }
    }
}
