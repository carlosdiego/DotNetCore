using MediatR;
using Shared.Pipeline;

namespace Shared.Request
{
    public class CreateUserRequest : IRequest<OperationResult>, IValidatable
    {
        public string Name { get; set; }

    }
}
