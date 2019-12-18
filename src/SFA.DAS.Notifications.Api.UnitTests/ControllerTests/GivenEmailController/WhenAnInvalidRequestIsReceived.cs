using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Controllers;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Orchestrators;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.Notifications.Api.UnitTests.ControllerTests.GivenEmailController
{
    public class WhenAnInvalidRequestIsReceived
    {
        [Test]
        public async Task ThenShouldReturnHttpBadRequestStatus()
        {
            var orchestratorResponse = new OrchestratorResponse { Code = NotificationOrchestratorCodes.Post.ValidationFailure };
            var mockNotificationOrchestrator = new Mock<INotificationOrchestrator>();
            mockNotificationOrchestrator.Setup(x => x.SendEmail(It.IsAny<Email>())).Returns(Task.FromResult(orchestratorResponse));

            var sut = new EmailController(mockNotificationOrchestrator.Object);
            sut.ControllerContext = TestHelpers.CreateControllerContextWithUser();

            var request = new Email();
            HttpResponseMessage controllerResponse = await sut.Post(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, controllerResponse.StatusCode);
        }
    }
}
