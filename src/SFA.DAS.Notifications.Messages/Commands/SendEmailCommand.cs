using System.Collections.Generic;

namespace SFA.DAS.Notifications.Messages.Commands
{
    public class SendEmailCommand
    {
        public string TemplateId { get; }
        public string RecipientsAddress { get; }
        public IReadOnlyDictionary<string, string> Tokens { get; }

        public SendEmailCommand(string templateId, string recipientsAddress, IReadOnlyDictionary<string, string> tokens)
        {
            TemplateId = templateId;
            RecipientsAddress = recipientsAddress;
            Tokens = tokens;
        }
    }
}
