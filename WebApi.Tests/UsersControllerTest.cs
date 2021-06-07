using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shared;
using Shared.Request;
using Shared.ViewModel;
using System.Threading.Tasks;
using WebApi.Controllers;
using Xunit;

namespace WebApi.Tests
{
    public class UsersControllerTest
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;
        private readonly UserController _sut;

        public UsersControllerTest()
        {
            _mediator = Substitute.For<IMediator>();
            _logger = Substitute.For<ILogger<UserController>>();
            _sut = new UserController(_mediator, _logger);
        }

        [Fact]
        public async Task When_Send_Request_For_Create_User_Then_Should_Be_Success()
        {
            //Arrange
            var request = new CreateUserRequest() { Name = "Create User API" };

            _mediator.Send(request).Returns(OperationResult.Success());
            //Act
            var response = await _sut.AddUser(request);
            var statusCodeResult = (IStatusCodeActionResult)response;
            //Assert
            Assert.True(statusCodeResult.StatusCode == StatusCodes.Status200OK);
        }

        [Fact]
        public async Task When_Send_Request_For_Get_User_by_Name_Then_Should_Be_Success()
        {
            //Arrange
            var request = new GetUsersRequest();
            request.Name = "test";


            var userSimpleGetResultViewModel = new UserViewModel[]
            {
                new UserViewModel
                {
                    Name = "test"
                }
            };

            _mediator.Send(request).Returns(OperationResult.Success(userSimpleGetResultViewModel));
            //Act
            var response = await _sut.GetUsersByName(request);
            var statusCodeResult = (IStatusCodeActionResult)response;
            //Assert
            Assert.True(statusCodeResult.StatusCode == StatusCodes.Status200OK);
        }
    }
}
