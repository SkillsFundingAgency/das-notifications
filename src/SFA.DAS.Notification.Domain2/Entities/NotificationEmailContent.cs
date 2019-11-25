using System.Collections.Generic;

namespace SFA.DAS.Notifications.Domain2.Entities
{
    public class NotificationEmailContent
    {
        public string Subject { get; set; }
        public string RecipientsAddress { get; set; }
        public string ReplyToAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}
