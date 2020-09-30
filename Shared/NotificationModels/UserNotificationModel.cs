using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.NotificationModels
{
    public class UserNotificationModel : INotification
    {
        public string Name { get; set; }

        public UserNotificationModel()
        { }

        public UserNotificationModel(string firstName) => Name = firstName;
    }
}
