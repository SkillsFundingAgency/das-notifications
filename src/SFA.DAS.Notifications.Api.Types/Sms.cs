using System.Collections.Generic;

namespace SFA.DAS.Notifications.Api.Types
{
    public class Sms
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
        /// The phone number to send the SMS to
        /// </summary>
        public string RecipientsNumber { get; set; }
        /// <summary>
        /// Keys + values of tokens to substitute in the template text
        /// </summary>
        public Dictionary<string, string> Tokens { get; set; }
    }
}