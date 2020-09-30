using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared;
using Shared.Exceptions;

namespace WebApi.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    { 
        protected readonly IMediator _mediator;
        protected readonly ILogger _logger;
        protected ApiControllerBase(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        protected async Task<IActionResult> SendCommand<T>(IRequest<OperationResult<T>> request)
        {
#if DEBUG
            _logger.LogInformation($"Request: {JsonConvert.SerializeObject(request)}");
#endif

            var (isSuccess, result, error) = await _mediator.Send(request).ConfigureAwait(false);
#if DEBUG
            _logger.LogInformation($"Result: {JsonConvert.SerializeObject(result)}");
#endif

            if (isSuccess)
                return Ok(result);

            if (error is CustomValidationFailedException errorValidation)
            {
                _logger.LogError($"Error: {JsonConvert.SerializeObject(errorValidation)}");
                return BadRequest(errorValidation.Errors);
            }

            _logger.LogError($"Error: {JsonConvert.SerializeObject(error)}");
            return BadRequest(new[] { new[] { error.Message } });
        }

        protected async Task<IActionResult> SendCommand(IRequest<OperationResult> request, int statusCode = 200)
        {
#if DEBUG
            _logger.LogInformation($"Request: {JsonConvert.SerializeObject(request)}");
#endif

            var (isSuccess, error) = await _mediator.Send(request);

            if (isSuccess)
                return StatusCode(statusCode);

            if (error is CustomValidationFailedException errorValidation)
            {
                _logger.LogError($"Error: {JsonConvert.SerializeObject(errorValidation)}");
                return BadRequest(errorValidation.Errors);
            }

            _logger.LogError($"Error: {JsonConvert.SerializeObject(error)}");
            return BadRequest(new[] { new[] { error.Message } });
        }
    }
}
