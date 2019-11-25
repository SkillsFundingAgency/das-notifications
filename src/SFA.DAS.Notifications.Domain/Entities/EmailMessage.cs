using System.Collections.Generic;

namespace SFA.DAS.Notifications.Domain2.Entities
{
    public class EmailMessage
    {
        public string SystemId { get; set; }
        public string TemplateId { get; set; }
        public string Subject { get; set; }
        public string RecipientsAddress { get; set; }
        public string ReplyToAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
        public string Reference { get; set; }
    }
}
