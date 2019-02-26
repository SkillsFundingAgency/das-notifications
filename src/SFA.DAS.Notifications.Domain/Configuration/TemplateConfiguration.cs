using System.Collections.Generic;

namespace SFA.DAS.Notifications.Domain.Configuration
{
    public class TemplateConfiguration
    {
        public List<Template> EmailServiceTemplates { get; set; }
        public List<SmsTemplate> SmsServiceTemplates { get; set; } = new List<SmsTemplate>();
    }
}
