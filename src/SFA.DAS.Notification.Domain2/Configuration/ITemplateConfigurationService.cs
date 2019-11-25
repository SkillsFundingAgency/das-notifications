using System.Threading.Tasks;
using SFA.DAS.Notifications.Domain.Configuration;

namespace SFA.DAS.Notifications.Domain2.Configuration
{
    public interface ITemplateConfigurationService
    {
        TemplateConfiguration Get();
        Task<TemplateConfiguration> GetAsync();
    }
}