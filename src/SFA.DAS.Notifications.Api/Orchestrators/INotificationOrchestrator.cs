using System;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Models;

namespace SFA.DAS.Notifications.Api.Orchestrators
{
    public interface INotificationOrchestrator
    {
        Task<OrchestratorResponse> SendEmail(SendEmailRequest notification);
    }
}