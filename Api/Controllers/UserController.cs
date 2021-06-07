using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Request;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ApiControllerBase
    {
        public UserController(IMediator mediator, ILogger<UserController> logger)
            : base(mediator, logger)
        { }


        [HttpPost("[action]")]
        public Task<IActionResult> AddUser([FromBody] CreateUserRequest request)        
           => SendCommand(request);


        [HttpGet("[action]")]
        public Task<IActionResult> GetUsers()
           => SendCommand(new GetUsersRequest());
    }
}
