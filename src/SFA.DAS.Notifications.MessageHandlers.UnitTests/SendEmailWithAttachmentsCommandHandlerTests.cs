using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using System.Threading;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SFA.DAS.Notifications.MessageHandlers.CommandHandlers;

namespace SFA.DAS.Notifications.MessageHandlers.UnitTests
{
    [TestFixture]
    public class SendEmailWithAttachmentsCommandHandlerTests
    {
        private Dictionary<string, string> _tokenDictionary;
        private Dictionary<string, byte[]> _attachmentsDictionary;
        private SendEmailWithAttachmentsCommandHandler _handler;
        private Mock<IMediator> _mediator;
        private Mock<ILogger<SendEmailWithAttachmentsCommandHandler>> _logger;
        private Messages.Commands.SendEmailWithAttachmentsCommand _message;

        [SetUp]
        public void Arrange()
        {
            _tokenDictionary = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            _attachmentsDictionary = new Dictionary<string, byte[]>
            {
                { "file1", new byte[5] },
                { "file2", new byte[10] }
            };

            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<SendEmailWithAttachmentsCommandHandler>>();

            _handler = new SendEmailWithAttachmentsCommandHandler(_mediator.Object, _logger.Object);

            _message = new Messages.Commands.SendEmailWithAttachmentsCommand(
                "templateId", 
                "to@test.com", 
                new ReadOnlyDictionary<string, string>(_tokenDictionary),
                new ReadOnlyDictionary<string, byte[]>(_attachmentsDictionary));
        }

        [Test]
        public async Task Handle_Should_Send_Email_With_Correct_Template_And_Email_Details()
        {
            await _handler.Handle(_message, Mock.Of<IMessageHandlerContext>());

            _mediator.Verify(x => 
                x.Send(It.Is<SendEmailMediatRCommand>(c => 
                         c.TemplateId == _message.TemplateId && 
                         c.RecipientsAddress == _message.RecipientsAddress), 
                       It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Handle_Should_Send_Email_With_Correctly_Mapped_Tokens()
        {
            await _handler.Handle(_message, Mock.Of<IMessageHandlerContext>());

            _mediator.Verify(x => 
                x.Send(It.Is<SendEmailMediatRCommand>(c => 
                         c.Tokens.ContainsKey("key1") && c.Tokens["key1"] == "value1" && 
                         c.Tokens.ContainsKey("key2") && c.Tokens["key2"] == "value2" &&
                         c.Tokens.Count == 2), 
                       It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Handle_Should_Send_Email_With_Correctly_Mapped_Attachments()
        {
            await _handler.Handle(_message, Mock.Of<IMessageHandlerContext>());

            _mediator.Verify(x =>
                x.Send(It.Is<SendEmailMediatRCommand>(c => 
                         c.Attachments["file1"].Length == 5 &&
                         c.Attachments["file2"].Length == 10 &&
                         c.Attachments.Count == 2), 
                       It.IsAny<CancellationToken>()));
        }

        [Test]
        public void Handle_Rethrow_Exception_When_Send_Fails()
        {
            _mediator.Setup(x => x.Send(It.IsAny<SendEmailMediatRCommand>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new InvalidCastException());

            Assert.ThrowsAsync<InvalidCastException>(() => _handler.Handle(_message, Mock.Of<IMessageHandlerContext>()));
        }
    }
}