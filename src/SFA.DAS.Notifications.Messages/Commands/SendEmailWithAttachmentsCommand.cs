using NServiceBus;
using System.Collections.Generic;

namespace SFA.DAS.Notifications.Messages.Commands
{
    public class SendEmailWithAttachmentsCommand
    {
        public string TemplateId { get; set; }
        public string RecipientsAddress { get; set; }
        public IReadOnlyDictionary<string, string> Tokens { get; set; }
        public DataBusProperty<IReadOnlyDictionary<string,byte[]>> Attachments { get; set; }

        public SendEmailWithAttachmentsCommand(string templateId,
                                               string recipientsAddress,
                                               IReadOnlyDictionary<string, string> tokens,
                                               DataBusProperty<IReadOnlyDictionary<string, byte[]>> attachments)
        {
            TemplateId = templateId;
            RecipientsAddress = recipientsAddress;
            Tokens = tokens;
            Attachments = attachments;
        }
    }
}