using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Orchestrators;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
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

            _orchestrator = new NotificationOrchestrator(_mediator.Object, Mock.Of<ILog>());

            _sms = new Sms
            {
                SystemId = Guid.NewGuid().ToString(),
                TemplateId = "MyTemplate",
                RecipientsNumber = "999",
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
            _mediator.Verify(m => m.SendAsync(It.Is<SendSmsCommand>(q => q.SystemId == _sms.SystemId)), Times.Once);
            _mediator.Verify(m => m.SendAsync(It.Is<SendSmsCommand>(q => q.TemplateId == _sms.TemplateId)), Times.Once);
            _mediator.Verify(m => m.SendAsync(It.Is<SendSmsCommand>(q => q.RecipientsNumber == _sms.RecipientsNumber)), Times.Once);
            _mediator.Verify(m => m.SendAsync(It.Is<SendSmsCommand>(q => q.Tokens.ContainsKey("Key") && q.Tokens["Key"] == "Value")), Times.Once);
        }

    }
}
