using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Controllers;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Orchestrators;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.Notifications.Api.UnitTests.ControllerTests.GivenEmailController
{
    public class WhenAValidRequestIsReceived
    {
        [Test]
        public async Task ThenShouldReturnHttpOKStatus()
        {
            var orchestratorResponse = new OrchestratorResponse { Code = NotificationOrchestratorCodes.Post.Success };
            var mockNotificationOrchestrator = new Mock<INotificationOrchestrator>();
            mockNotificationOrchestrator.Setup(x => x.SendSms(It.IsAny<Sms>())).Returns(Task.FromResult(orchestratorResponse));
            
            var sut = new SmsController(mockNotificationOrchestrator.Object);
            sut.ControllerContext = TestHelpers.CreateControllerContextWithUser();
            
            var request = new Sms();
            HttpResponseMessage controllerResponse = await sut.Post(request);

            Assert.AreEqual(HttpStatusCode.OK, controllerResponse.StatusCode);
        }
    }
}
