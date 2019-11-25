using System.Collections.Generic;

namespace SFA.DAS.Notifications.Messages2.Commands
{
    public class SendSmsCommand
    {
        public string SystemId { get; set; }
        public string TemplateId { get; set; }
        public string RecipientsNumber { get; set; }
        public Dictionary<string,string> Tokens { get; set; }

        public SendSmsCommand(string systemId, string templateId, string recipientsNumber, Dictionary<string,string> tokens)
        {
            SystemId = systemId;
            TemplateId = templateId;
            RecipientsNumber = recipientsNumber;
            Tokens = tokens;
        }
    }
}
