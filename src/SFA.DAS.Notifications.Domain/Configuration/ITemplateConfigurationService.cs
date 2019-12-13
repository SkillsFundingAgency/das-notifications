using System.Threading.Tasks;

namespace SFA.DAS.Notifications.Domain.Configuration
{
    public interface ITemplateConfigurationService
    {
        TemplateConfiguration Get();
    }
}