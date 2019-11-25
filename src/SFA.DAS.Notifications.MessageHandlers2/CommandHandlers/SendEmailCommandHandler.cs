using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SendEmailCommand = SFA.DAS.Notifications.Application.Commands.SendEmail.SendEmailCommand;


namespace SFA.DAS.Notifications.MessageHandlers2.CommandHandlers
{
    public class SendEmailCommandHandler : IHandleMessages<Messages.Commands.SendEmailCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public SendEmailCommandHandler(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(Messages.Commands.SendEmailCommand message, IMessageHandlerContext context)
        {
            try
            {
                var command = new SendEmailCommand {
                    SystemId = "X",
                    TemplateId = message.TemplateId,
                    Subject = message.Subject,
                    RecipientsAddress = message.RecipientsAddress,
                    ReplyToAddress = message.ReplyToAddress,
                    Tokens = message.Tokens.ToDictionary(e => e.Key, e => e.Value)
                };

                await _mediator.SendAsync(command);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }

        }
    }
}