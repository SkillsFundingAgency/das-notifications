using System.Threading.Tasks;

namespace SFA.DAS.Notifications.Domain2.Configuration
{
    public interface ITemplateConfigurationService
    {
        TemplateConfiguration Get();
        Task<TemplateConfiguration> GetAsync();
    }
}