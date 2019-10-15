using System;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.Notifications.Application.Commands.SendEmail;

namespace SFA.DAS.Notifications.MessageHandlers.CommandHandlers
{
    public class SendEmailCommandHandler : IHandleMessages<SendEmailCommand>
    {
        private readonly IMediator _mediator;

        public SendEmailCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(SendEmailCommand message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
