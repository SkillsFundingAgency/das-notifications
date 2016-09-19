﻿using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Notifications.Application.Commands.SendSms
{
    public class SendSmsCommand : IAsyncRequest
    {
        public string SystemId { get; set; }
        public string TemplateId { get; set; }
        public string RecipientsNumber { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}