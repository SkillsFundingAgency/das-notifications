using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.Notifications.Api.Attributes;
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
        [ApiAuthorize(Roles = "SendSMS")]
        public async Task<HttpResponseMessage> Post(SendSmsRequest notification)
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);

            //notification.SystemId = User.Identity.Name;

            //await _orchestrator.SendSms(notification);

            //return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
