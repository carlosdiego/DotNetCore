using Core.Repositories;
using Domain.Models;
using Domain.Repositories;
using MediatR;
using Shared;
using Shared.NotificationModels;
using Shared.Request;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.RequestHandlers
{
    public sealed class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, OperationResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public CreateUserRequestHandler(IUserRepository userRepository, IMediator mediator)
        { _userRepository = userRepository; _mediator = mediator; }

        public async Task<OperationResult> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.InsertAsync(new User() { Name = request.Name });
                await _mediator.Publish(new UserNotificationModel(request.Name));

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Error(ex);
            }
        }
    }
}
