using System;
using Microsoft.WindowsAzure.Storage.Table;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Infrastructure.AzureMessageNotificationRepository
{
    public class NotificationTableEntity : TableEntity
    {
        public NotificationTableEntity(NotificationFormat notificationFormat, string messageId) : base(notificationFormat.ToString(), messageId) {}

        // ReSharper disable once UnusedMember.Global
        public NotificationTableEntity() {}

        public string Data { get; set; }
    }
}
