using System.Collections.Generic;

namespace SFA.DAS.Notifications.Domain.Entities
{
    public class EmailMessage
    {
        public string TemplateId { get; set; }
        public string RecipientsAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
        public Dictionary<string, byte[]> Attachments { get; set; }
        public string Reference { get; set; }
    }
}
