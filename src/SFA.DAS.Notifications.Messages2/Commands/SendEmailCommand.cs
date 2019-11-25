﻿using System.Collections.Generic;

namespace SFA.DAS.Notifications.Messages2.Commands
{
    public class SendEmailCommand
    {
        public string TemplateId { get; }
        public string RecipientsAddress { get; }
        public string ReplyToAddress { get; }
        public IReadOnlyDictionary<string, string> Tokens { get; }
        public string Subject { get; set; }

        public SendEmailCommand(string templateId, string recipientsAddress, string replyToAddress, IReadOnlyDictionary<string, string> tokens, string subject)
        {
            TemplateId = templateId;
            RecipientsAddress = recipientsAddress;
            ReplyToAddress = replyToAddress;
            Tokens = tokens;
            Subject = subject;
        }
    }
}
