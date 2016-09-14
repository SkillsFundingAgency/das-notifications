﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.Notifications.Application.Messages
{
    public class SmsMessage
    {
        public string UserId { get; set; }
        public string MessageType { get; set; }
        public string TemplateId { get; set; }
        public string SendTo { get; set; }
        public string ReplyTo { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}
