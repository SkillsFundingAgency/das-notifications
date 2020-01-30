using System.Threading.Tasks;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Api.Core;

namespace SFA.DAS.Notifications.Api.Orchestrators
{
    public interface INotificationOrchestrator
    {
        Task<OrchestratorResponse> SendEmail(Email request);
        Task<OrchestratorResponse> SendSms(Sms request);
    }
}