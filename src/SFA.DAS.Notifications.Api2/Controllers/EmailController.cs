using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Api2.Orchestrators;

namespace SFA.DAS.Notifications.Api2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly INotificationOrchestrator _orchestrator;

        public EmailController(INotificationOrchestrator orchestrator)
        {
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
        }

        [HttpPost]
        [Route("")]
        //[Authorize(Roles = "SendEmail")] todo roles
        //[Authorize]
        public async Task<HttpResponseMessage> Post(Email notification)
        {
            if (string.IsNullOrEmpty(notification.SystemId)
                && !string.IsNullOrEmpty(User.Identity.Name))
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
