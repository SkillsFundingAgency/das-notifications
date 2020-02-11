using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Orchestrators;

namespace SFA.DAS.Notifications.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly INotificationOrchestrator _orchestrator;

        public SmsController(INotificationOrchestrator orchestrator)
        {
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "SendSMS")]
        public async Task<HttpResponseMessage> Post(Sms notification)
        {
            if (string.IsNullOrEmpty(notification.SystemId)
                && !string.IsNullOrEmpty(User.Identity.Name))
            {
                notification.SystemId = User.Identity.Name;
            }

            OrchestratorResponse result = await _orchestrator.SendSms(notification);
            if (result.Code == NotificationOrchestratorCodes.Post.ValidationFailure)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}