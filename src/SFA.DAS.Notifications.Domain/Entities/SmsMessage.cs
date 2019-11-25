using System.Collections.Generic;

namespace SFA.DAS.Notifications.Domain.Entities
{
    public class SmsMessage
    {
        public string SystemId { get; set; }
        public string TemplateId { get; set; }
        public string RecipientsNumber { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
        public string Reference { get; set; }
    }
}
