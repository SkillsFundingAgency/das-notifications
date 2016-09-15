using System;
using Microsoft.WindowsAzure.Storage.Table;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Infrastructure.AzureMessageNotificationRepository
{
    public class EmailMessageTableEntity : TableEntity
    {
        public EmailMessageTableEntity(string messageId) : base(NotificationFormat.Email.ToString(), messageId) {}

        // ReSharper disable once UnusedMember.Global
        public EmailMessageTableEntity() {}

        public string Data { get; set; }
    }
}
