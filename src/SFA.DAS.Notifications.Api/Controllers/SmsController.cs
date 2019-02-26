using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.Notifications.Api.Attributes;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Orchestrators;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.Notifications.Api.Controllers
{
    [RoutePrefix("api/sms")]
    public class SmsController : ApiController
    {
        private readonly INotificationOrchestrator _orchestrator;

        public SmsController(INotificationOrchestrator orchestrator)
        {
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
        }

        [Route("")]
        [ApiAuthorize(Roles = "SendSMS")]
        public async Task<HttpResponseMessage> Post(Sms notification)
        {
            if (!string.IsNullOrEmpty(User.Identity.Name))
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
