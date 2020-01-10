﻿using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Notifications.Application.Commands.SendEmail
{
    public class SendEmailMediatRCommand : IRequest
    {
        public string SystemId { get; set; }
        public string TemplateId { get; set; }
        public string Subject { get; set; }
        public string RecipientsAddress { get; set; }
        public string ReplyToAddress { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}