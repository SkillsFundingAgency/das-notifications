using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
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
        [Authorize(Roles = "SendEmail")]
        public async Task<HttpResponseMessage> Post(Email notification)
        {
            notification.SystemId = User.Identity.Name;

            await _orchestrator.SendEmail(notification);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
