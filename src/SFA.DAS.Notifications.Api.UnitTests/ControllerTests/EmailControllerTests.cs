using System;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Controllers;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Models;
using SFA.DAS.Notifications.Api.Orchestrators;

namespace SFA.DAS.Notifications.Api.UnitTests.ControllerTests
{
    [TestFixture]
    public class EmailControllerTests
    {
        private Mock<INotificationOrchestrator> _orchestrator;
        private EmailController _controller;

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<INotificationOrchestrator>();

            _controller = new EmailController(_orchestrator.Object);
        }

        [TestCase(NotificationOrchestratorCodes.Post.Success, HttpStatusCode.OK)]
        [TestCase(NotificationOrchestratorCodes.Post.ValidationFailure, HttpStatusCode.BadRequest)]
        public async Task ReturnExpectedStatusCodeForEmailPost(string responseCode, HttpStatusCode statusCode)
        {
            var response = new OrchestratorResponse
            {
                Code = responseCode
            };

            _orchestrator.Setup(x => x.SendEmail(It.IsAny<EmailViewModel>())).ReturnsAsync(response);

            var httpResponse = await _controller.Post(new EmailViewModel());

            Assert.That(httpResponse.StatusCode, Is.EqualTo(statusCode));
        }
    }
}