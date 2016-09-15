using System;
using System.Collections.Generic;

namespace SFA.DAS.Notifications.Domain.Entities
{
    public class SmsNotificationContent
    {
        public string RecipientsNumber { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}
