using MediatR;
using Shared.Pipeline;
using Shared.ViewModel;

namespace Shared.Request
{
    public class GetUsersRequest : IRequest<OperationResult<UserViewModel[]>>
    {
    }
}
