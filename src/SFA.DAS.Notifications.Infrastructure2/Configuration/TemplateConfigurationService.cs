using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Configuration;
using SFA.DAS.Notifications.Domain2.Configuration;

namespace SFA.DAS.Notifications.Infrastructure2.Configuration
{
    public class TemplateConfigurationService : ITemplateConfigurationService
    {
        protected const string ServiceName = "SFA.DAS.Notifications-Templates";
        private const string Version = "1.0";

        private readonly IConfigurationRepository _configurationRepository;
        private readonly string _environmentName;

        public TemplateConfigurationService(IConfigurationRepository configurationRepository, string environmentName)
        {
            _configurationRepository = configurationRepository;
            _environmentName = environmentName;
        }

        public TemplateConfiguration Get()
        {
            var json = _configurationRepository.Get(ServiceName, _environmentName, Version);
            return JsonConvert.DeserializeObject<TemplateConfiguration>(json);
        }

        public async Task<TemplateConfiguration> GetAsync()
        {
            var json = await _configurationRepository.GetAsync(ServiceName, _environmentName, Version);
            return JsonConvert.DeserializeObject<TemplateConfiguration>(json);
        }
    }
}
