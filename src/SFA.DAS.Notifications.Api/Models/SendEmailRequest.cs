using System.Collections.Generic;

namespace SFA.DAS.Notifications.Api.Models
{
    public class SendEmailRequest
    {
        public string SystemId { get; set; }
        public string TemplateId { get; set; }
        public string Subject { get; set; }
        public string RecipientsAddress { get; set; }
        public string ReplyToAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}
