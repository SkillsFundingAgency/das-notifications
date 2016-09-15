﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.Notifications.Domain.Entities
{
    public class SmsMessage
    {
        public string SystemId { get; set; }
        public string TemplateId { get; set; }
        public string SendTo { get; set; }
        public string ReplyTo { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
    }
}
