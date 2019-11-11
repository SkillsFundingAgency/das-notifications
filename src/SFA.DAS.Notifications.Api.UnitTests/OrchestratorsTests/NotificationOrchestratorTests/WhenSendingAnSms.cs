using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Orchestrators;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Application.Commands.SendSms;

namespace SFA.DAS.Notifications.Api.UnitTests.OrchestratorsTests.NotificationOrchestratorTests
{
    public class WhenSendingAnSms
    {
        private Mock<IMediator> _mediator;
        private NotificationOrchestrator _orchestrator;
        private Sms _sms;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _orchestrator = new NotificationOrchestrator(Mock.Of<ILog>(), Mock.Of<IMessageSession>());

            _sms = new Sms {
                SystemId = Guid.NewGuid().ToString(),
                TemplateId = "MyTemplate",
                RecipientsNumber = "299792458",
                Tokens = new Dictionary<string, string>
                {
                    { "Key", "Value" }
                }
            };
        }

        [Test]
        public async Task ThenItShouldReturnSuccessResponse()
        {
            // Act
            var actual = await _orchestrator.SendSms(_sms);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(NotificationOrchestratorCodes.Post.Success, actual.Code);
        }

        [Test]
        public async Task ThenItShouldSendAnSms()
        {
            // Act
            await _orchestrator.SendSms(_sms);

            // Assert
            _mediator.Verify(m => m.SendAsync(
                It.Is<SendSmsCommand>(q =>
                    q.SystemId == _sms.SystemId
                    && q.TemplateId == _sms.TemplateId
                    && q.RecipientsNumber == _sms.RecipientsNumber
                    && q.Tokens["Key"] == "Value"))
                , Times.Once);
        }

    }
}
