using System;
using MediatR;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Application.Commands.DispatchNotification
{
    public class DispatchNotificationCommand : IAsyncRequest
    {
        public NotificationFormat Format { get; set; }
        public string MessageId { get; set; }
    }
}
