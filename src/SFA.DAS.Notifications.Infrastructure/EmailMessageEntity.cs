using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Notifications.Infrastructure
{
    public class EmailMessageEntity : TableEntity
    {
        public EmailMessageEntity(string userId, string messageId)
            : base(userId, messageId)
        {
        }

        public EmailMessageEntity() { }

        public string Data { get; set; }
    }
}