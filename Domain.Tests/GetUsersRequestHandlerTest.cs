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
    public class GetUsersRequestHandlerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly GetUsersRequestHandler _sut;

        public GetUsersRequestHandlerTest()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _sut = new GetUsersRequestHandler(_userRepository);
        }

        [Fact]
        public void When_Get_Users_Then_Should_Be_Success()
        {
            var request = new GetUsersRequest();

            var result = _sut.Handle(request, new CancellationToken()).GetAwaiter().GetResult();

            Assert.True(result.IsSuccess);
        }
    }
}
