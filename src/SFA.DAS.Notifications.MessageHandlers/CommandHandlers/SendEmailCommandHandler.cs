using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Notifications.Application.Commands.SendEmail;


namespace SFA.DAS.Notifications.MessageHandlers.CommandHandlers
{
    public class SendEmailCommandHandler : IHandleMessages<Messages.Commands.SendEmailCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SendEmailCommandHandler> _logger;

        public SendEmailCommandHandler(IMediator mediator, ILogger<SendEmailCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(Messages.Commands.SendEmailCommand message, IMessageHandlerContext context)
        {
            try
            {
                var command = new SendEmailMediatRCommand {
                    TemplateId = message.TemplateId,
                    RecipientsAddress = message.RecipientsAddress,
                    Tokens = message.Tokens.ToDictionary(e => e.Key, e => e.Value)
                };

                await _mediator.Send(command);

                await context.Reply(new Messages.Commands.SendEmailCommandSuccess(message));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }

        }
    }
}