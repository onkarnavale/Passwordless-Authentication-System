using Fido2Apis.Application.Response;
using Microsoft.AspNetCore.Mvc;

namespace Fido2Apis.Application.Apis
{
    public class BaseController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ProcessApiResponse(ResponseObject responseObject)
        {
            bool success = responseObject.Success;
            string message = responseObject.Message;
            int code =responseObject.code;

            ResponseObject processedResponseObject = new ResponseObject();

            switch (code)
            {
                case StatusCodes.Status200OK:
                    processedResponseObject.Success = success;
                    processedResponseObject.Message = message;
                    processedResponseObject.Data = responseObject.Data;
                    processedResponseObject.code = code;

                    return Ok(processedResponseObject);

                case StatusCodes.Status201Created:
                    processedResponseObject.Success = success;
                    processedResponseObject.Message = message;
                    processedResponseObject.Data = responseObject.Data;
                    processedResponseObject.code = code;

                    return Ok(processedResponseObject);

                case StatusCodes.Status400BadRequest:
                    processedResponseObject.Success = success;
                    processedResponseObject.Message = message;
                    processedResponseObject.code = code;

                    return BadRequest(processedResponseObject);

                case StatusCodes.Status401Unauthorized:
                    processedResponseObject.Success = success;
                    processedResponseObject.Message = message;
                    processedResponseObject.code = code;

                    return Unauthorized(processedResponseObject);

                case StatusCodes.Status403Forbidden:
                    return Forbid();

                case StatusCodes.Status404NotFound:
                    processedResponseObject.Success = success;
                    processedResponseObject.Message = message;
                    processedResponseObject.code = code;

                    return NotFound(processedResponseObject);

                case StatusCodes.Status409Conflict:
                    processedResponseObject.Success = success;
                    processedResponseObject.Message = message;
                    processedResponseObject.code = code;

                    return Conflict(processedResponseObject);

                case StatusCodes.Status500InternalServerError:
                    processedResponseObject.Success = success;
                    processedResponseObject.Message = message;
                    processedResponseObject.code = code;

                    return StatusCode(StatusCodes.Status500InternalServerError, processedResponseObject);

                default:
                    processedResponseObject.Success = success;
                    processedResponseObject.Message = message;
                    processedResponseObject.code = code;

                    return BadRequest(processedResponseObject);
            }
        }
    }
}
