using System.Threading.Tasks;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Api2.Core;

namespace SFA.DAS.Notifications.Api2.Orchestrators
{
    public interface INotificationOrchestrator
    {
        Task<OrchestratorResponse> SendEmail(Email request);
        Task<OrchestratorResponse> SendSms(Sms request);
    }
}