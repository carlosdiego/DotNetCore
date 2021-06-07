using Domain.Models;
using Domain.Repositories;
using MediatR;
using Shared;
using Shared.Request;
using Shared.ViewModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.RequestHandlers
{
    public sealed class GetUsersRequestHandler : IRequestHandler<GetUsersRequest, OperationResult<UserViewModel[]>>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersRequestHandler(IUserRepository userRepository)
        { _userRepository = userRepository;}

        public Task<OperationResult<UserViewModel[]>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
        {           
            var users = _userRepository.GetAllProjected<UserViewModel>().ToArray();

            return OperationResult.Success(users).AsTask;
        }
    }
}
