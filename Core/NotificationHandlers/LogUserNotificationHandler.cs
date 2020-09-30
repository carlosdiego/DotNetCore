using MediatR;
using Shared.NotificationModels;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.NotificationHandlers
{
    public class LogUserNotificationHandler : INotificationHandler<UserNotificationModel>
    {
        public Task Handle(UserNotificationModel notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
