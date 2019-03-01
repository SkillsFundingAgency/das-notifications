using System.Collections.Generic;

namespace SFA.DAS.Notifications.Api.Types
{
    public class Email
    {
        /// <summary>
        /// The Id of the system that requested the message to be sent
        /// </summary>
        public string SystemId { get; set; }
        /// <summary>
        /// The Id of the message to send
        /// </summary>
        public string TemplateId { get; set; }
        /// <summary>
        /// The text to use as the email subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// The email address of the recipient
        /// </summary>
        public string RecipientsAddress { get; set; }
        /// <summary>
        /// The email address to use as the sender/reply address
        /// </summary>
        public string ReplyToAddress { get; set; }
        /// <summary>
        /// Keys + values of tokens to substitute in the template text
        /// </summary>
        public Dictionary<string, string> Tokens { get; set; }

        public Email() { }

        public Email(
            string systemId,
            string templateId,
            string subject,
            string recipientsAddress,
            string replyToAddress,
            Dictionary<string, string> tokens)
        {
            SystemId = systemId;
            TemplateId = templateId;
            Subject = subject;
            RecipientsAddress = recipientsAddress;
            ReplyToAddress = replyToAddress;
            Tokens = tokens;
        }
    }
}