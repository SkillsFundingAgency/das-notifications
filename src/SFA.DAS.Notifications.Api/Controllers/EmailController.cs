using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Api.Orchestrators;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Notifications.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly INotificationOrchestrator _orchestrator;
        private readonly ILogger<EmailController> _logger;

        public EmailController(INotificationOrchestrator orchestrator, ILogger<EmailController> logger)
        {
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
            _logger = logger;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "SendEmail")]
        public async Task<HttpResponseMessage> Post(Email notification)
        {
            try
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
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
