using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Orchestrators;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Application.Commands.SendEmail;

namespace SFA.DAS.Notifications.Api.UnitTests.OrchestratorsTests.NotificationOrchestratorTests
{
    public class WhenSendingAnEmail
    {
        private Mock<IMediator> _mediator;
        private NotificationOrchestrator _orchestrator;
        private Email _email;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _orchestrator = new NotificationOrchestrator(_mediator.Object);

            _email = new Email
            {
                SystemId = Guid.NewGuid().ToString(),
                TemplateId = "MyTemplate",
                RecipientsAddress = "user.one@unit.tests",
                ReplyToAddress = "noreply@unit.tests",
                Subject = "Unit test email",
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
            var actual = await _orchestrator.SendEmail(_email);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(NotificationOrchestratorCodes.Post.Success, actual.Code);
        }

        [Test]
        public async Task ThenItShouldSendAnEmail()
        {
            // Act
            await _orchestrator.SendEmail(_email);

            // Assert
            _mediator.Verify(m => m.SendAsync(It.Is<SendEmailCommand>(q => q.SystemId == _email.SystemId)), Times.Once);
            _mediator.Verify(m => m.SendAsync(It.Is<SendEmailCommand>(q => q.RecipientsAddress == _email.RecipientsAddress)), Times.Once);
            _mediator.Verify(m => m.SendAsync(It.Is<SendEmailCommand>(q => q.ReplyToAddress == _email.ReplyToAddress)), Times.Once);
            _mediator.Verify(m => m.SendAsync(It.Is<SendEmailCommand>(q => q.Subject == _email.Subject)), Times.Once);
            _mediator.Verify(m => m.SendAsync(It.Is<SendEmailCommand>(q => q.Tokens.ContainsKey("Key") && q.Tokens["Key"] == "Value")), Times.Once);
        }

    }
}
