using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.Notifications.Api.Models;
using SFA.DAS.Notifications.Api.Orchestrators;

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
        public async Task<HttpResponseMessage> Post(SendEmailRequest notification)
        {
            var response = await _orchestrator.SendEmail(notification);

            if (response.Code == NotificationOrchestratorCodes.Post.ValidationFailure)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
