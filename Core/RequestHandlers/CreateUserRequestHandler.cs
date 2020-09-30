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

        public Task<OperationResult> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                 _userRepository.Insert(new User() { Name = request.Name });
                _mediator.Publish(new UserNotificationModel(request.Name));

                return Task.FromResult(OperationResult.Success());
            }
            catch (Exception ex)
            {
                return Task.FromResult(OperationResult.Error(ex));
            }
        }
    }
}
