using System.Threading.Tasks;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Models;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.Notifications.Api.Orchestrators
{
    public interface INotificationOrchestrator
    {
        Task<OrchestratorResponse> SendEmail(Email request);
        Task<OrchestratorResponse> SendSms(SendSmsRequest request);
    }
}