using MediatR;
using Shared.Pipeline;
using Shared.ViewModel;

namespace Shared.Request
{
    public class GetUsersRequest : IRequest<OperationResult<UserViewModel[]>>
    {
        public string Name { get; set; }

        public GetUsersRequest()
        {

        }
        public GetUsersRequest(string name)
        {
            Name = name;
        }
    }
}
