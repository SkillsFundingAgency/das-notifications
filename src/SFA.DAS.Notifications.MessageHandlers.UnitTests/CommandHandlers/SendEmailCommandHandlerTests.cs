using System.Threading.Tasks;
using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SendEmailCommandHandler = SFA.DAS.Notifications.MessageHandlers.CommandHandlers.SendEmailCommandHandler;

namespace SFA.DAS.Notifications.MessageHandlers.UnitTests.CommandHandlers
{
    [TestFixture]
    public class SendEmailCommandHandlerTests
    {
        private SendEmailCommandHandler _handler;
        private Mock<IMediator> _mediator;
        private SendEmailCommand _message;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _mediator.Setup(x => x.SendAsync(It.IsAny<SendEmailCommand>()));

            _handler = new SendEmailCommandHandler(_mediator.Object);

            _message = new SendEmailCommand();
        }

        [Test]
        public async Task Handle_Should_Send_Email()
        {
            await _handler.Handle(_message, Mock.Of<IMessageHandlerContext>());

            _mediator.Verify(x => x.SendAsync(It.Is<SendEmailCommand>(c => c.TemplateId == _message.TemplateId && c.RecipientsAddress == _message.RecipientsAddress)));
        }
    }
}
