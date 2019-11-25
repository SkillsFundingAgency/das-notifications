using System.Collections.Generic;

namespace SFA.DAS.Notifications.Domain2.Entities
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
