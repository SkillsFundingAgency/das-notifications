using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.Notifications.Domain.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public class TemplateConfigurationService : ITemplateConfigurationService
    {
        protected const string ServiceName = "SFA.DAS.Notifications-Templates";
        private const string Version = "1.0";

        private readonly IConfiguration _configuration;

        public TemplateConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TemplateConfiguration Get()
        {
            var json = _configuration[$"{ServiceName}_{Version}"];
            return JsonConvert.DeserializeObject<TemplateConfiguration>(json);
        }
    }
}
