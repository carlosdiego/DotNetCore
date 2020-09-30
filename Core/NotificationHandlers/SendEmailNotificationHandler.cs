using MediatR;
using Shared.NotificationModels;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.NotificationHandlers
{
    public class SendEmailNotificationHandler : INotificationHandler<UserNotificationModel>
    {
        public Task Handle(UserNotificationModel notification, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
