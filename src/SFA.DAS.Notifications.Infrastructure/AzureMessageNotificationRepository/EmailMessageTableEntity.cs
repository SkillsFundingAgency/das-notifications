using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Notifications.Infrastructure.AzureMessageNotificationRepository
{
    public class EmailMessageTableEntity : TableEntity
    {
        public EmailMessageTableEntity(string messageType, string messageId)
            : base(messageType, messageId) {}

        // ReSharper disable once UnusedMember.Global
        public EmailMessageTableEntity() {}

        public string Data { get; set; }
    }
}
