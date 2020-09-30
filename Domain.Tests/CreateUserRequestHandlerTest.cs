using Domain.Repositories;
using Domain.RequestHandlers;
using MediatR;
using NSubstitute;
using Shared.Request;
using System;
using System.Threading;
using Xunit;

namespace Domain.Tests
{
    public class CreateUserRequestHandlerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly CreateUserRequestHandler _sut;
        private readonly IMediator _mediator;

        public CreateUserRequestHandlerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _mediator = Substitute.For<IMediator>();
            _sut = new CreateUserRequestHandler(_userRepository, _mediator);
        }

        [Fact]
        public void When_Create_User_Then_Should_Be_Success()
        {
            var request = new CreateUserRequest
            {
                Name = "Teste Add User"
                
            };

            var result = _sut.Handle(request, new CancellationToken()).GetAwaiter().GetResult();

            Assert.True(result.IsSuccess);
        }
    }
}
