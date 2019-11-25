using System.Collections.Generic;

namespace SFA.DAS.Notifications.Domain2.Entities
{
    public class NotificationSmsContent
    {
        public string RecipientsNumber { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}
