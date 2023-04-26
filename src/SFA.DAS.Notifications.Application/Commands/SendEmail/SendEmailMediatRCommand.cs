using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Notifications.Application.Commands.SendEmail
{
    public class SendEmailMediatRCommand : IRequest
    {
        public string TemplateId { get; set; }
        public string RecipientsAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
        public Dictionary<string, byte[]> Attachments { get; set; } = new Dictionary<string, byte[]>();
    }
}
