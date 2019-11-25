﻿namespace SFA.DAS.Notifications.Domain2.Configuration
{
    public class SmsTemplate
    {
        public string Id { get; set; }
        public string ServiceId { get; set; }

        public SmsTemplate() { }
        public SmsTemplate(string id, string serviceId)
        {
            Id = id;
            ServiceId = serviceId;
        }

    }
}
