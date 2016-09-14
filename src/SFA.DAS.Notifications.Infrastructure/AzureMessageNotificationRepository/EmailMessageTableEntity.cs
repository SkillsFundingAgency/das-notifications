using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Notifications.Infrastructure.AzureMessageNotificationRepository
{
    public class EmailMessageTableEntity : TableEntity
    {
        public EmailMessageTableEntity(string userId, string messageId)
            : base(userId, messageId) {}

        public string Data { get; set; }
    }
}
