using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.Notifications.Api.Attributes;
using SFA.DAS.Notifications.Api.Orchestrators;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.Notifications.Api.Controllers
{
    [RoutePrefix("api/email")]
    public class EmailController : ApiController
    {
        private readonly INotificationOrchestrator _orchestrator;

        public EmailController(INotificationOrchestrator orchestrator)
        {
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
        }

        [Route("")]
        [ApiAuthorize(Roles = "SendEmail")]
        public async Task<HttpResponseMessage> Post(Email notification)
        {
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                notification.SystemId = User.Identity.Name;
            }
            
            var result = await _orchestrator.SendEmail(notification);
            if (result.Code == NotificationOrchestratorCodes.Post.ValidationFailure)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
