using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SendEmailCommandHandler = SFA.DAS.Notifications.MessageHandlers.CommandHandlers.SendEmailCommandHandler;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Notifications.MessageHandlers.UnitTests.CommandHandlers
{
    [TestFixture]
    public class SendEmailCommandHandlerTests
    {
        private Dictionary<string, string> dictionary;
        private SendEmailCommandHandler _handler;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Messages.Commands.SendEmailCommand _message;

        [SetUp]
        public void Arrange()
        {
            dictionary = new Dictionary<string, string>();
            dictionary.Add("key1", "value1");
            dictionary.Add("key2", "value2");

            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();

            _handler = new SendEmailCommandHandler(_mediator.Object, _logger.Object);

            _message = new Messages.Commands.SendEmailCommand("templateId", "to@test.com", "reply@test.com", new ReadOnlyDictionary<string, string>(dictionary));
        }

        [Test]
        public async Task Handle_Should_Send_Email_With_Correct_Template_And_Email_Details()
        {
            await _handler.Handle(_message, Mock.Of<IMessageHandlerContext>());

            _mediator.Verify(x => x.SendAsync(It.Is<SendEmailCommand>(c => c.TemplateId == _message.TemplateId && c.RecipientsAddress == _message.RecipientsAddress && c.ReplyToAddress == _message.ReplyToAddress)));
        }

        [Test]
        public async Task Handle_Should_Send_Email_With_Correctly_Mapped_Tokens()
        {
            await _handler.Handle(_message, Mock.Of<IMessageHandlerContext>());

            _mediator.Verify(x => x.SendAsync(It.Is<SendEmailCommand>(c => c.Tokens["key1"] == "value1" && c.Tokens["key2"] == "value2")));
        }

        [Test]
        public void Handle_Rethrow_Exception_When_Send_Fails()
        {
            _mediator.Setup(x => x.SendAsync(It.IsAny<SendEmailCommand>())).ThrowsAsync(new InvalidCastException());

            Assert.ThrowsAsync<InvalidCastException>( () => _handler.Handle(_message, Mock.Of<IMessageHandlerContext>()));
        }
    }
}