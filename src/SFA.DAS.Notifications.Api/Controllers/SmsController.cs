using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.Notifications.Api.Models;
using SFA.DAS.Notifications.Api.Orchestrators;

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
        [Authorize(Roles = "SendSMS")]
        public async Task<HttpResponseMessage> Post(SendSmsRequest notification)
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);

            //var response = await _orchestrator.SendSms(notification);

            //if (response.Code == NotificationOrchestratorCodes.Post.ValidationFailure)
            //    return new HttpResponseMessage(HttpStatusCode.BadRequest);

            //return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
