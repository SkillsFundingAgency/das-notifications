using System.Collections.Generic;

namespace SFA.DAS.Notifications.Api.Models
{
    public class SendSmsRequest
    {
        public string SystemId { get; set; }
        public string TemplateId { get; set; }
        public string RecipientsNumber { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}
