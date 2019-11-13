using System.Collections.Generic;

namespace SFA.DAS.Notifications.Messages.Commands
{
    public class SendEmailCommand
    {
        public string TemplateId { get; }
        public string RecipientsAddress { get; }
        public string ReplyToAddress { get; }
        public Dictionary<string, string> Tokens { get; }

        public SendEmailCommand(string templateId, string recipientsAddress, string replyToAddress, Dictionary<string, string> tokens)
        {
            TemplateId = templateId;
            RecipientsAddress = recipientsAddress;
            ReplyToAddress = replyToAddress;
            Tokens = tokens;
        }
    }
}
