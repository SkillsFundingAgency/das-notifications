using System;
using System.Collections.Generic;

namespace SFA.DAS.Notifications.Application.Messages
{
    public class EmailMessageContent
    {
        public string RecipientsAddress { get; set; }
        public string ReplyToAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}
