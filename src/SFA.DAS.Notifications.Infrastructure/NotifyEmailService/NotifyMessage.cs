using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public class NotifyMessage
    {
        public string To { get; set; }
        public string Template { get; set; }
        public Dictionary<string, string> Personalisation { get; set; }
        public Dictionary<string, byte[]> Attachments { get; set; }
        public string Reference { get; set; }
    }
}