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

        private readonly string _environmentName; // todo what about environment name
        private readonly IConfiguration _configuration;

        public TemplateConfigurationService(string environmentName, IConfiguration configuration)
        {
            _environmentName = environmentName;
            _configuration = configuration;
        }

        //todo config usage updated here
        public TemplateConfiguration Get()
        {
            var json = _configuration[$"{ServiceName}_{Version}"];
            return JsonConvert.DeserializeObject<TemplateConfiguration>(json);
        }

        [Obsolete("use non async")] //todo remove this
        public async Task<TemplateConfiguration> GetAsync()
        {
            var json = _configuration[$"{ServiceName}_{Version}"];
            return JsonConvert.DeserializeObject<TemplateConfiguration>(json);
        }
    }
}
