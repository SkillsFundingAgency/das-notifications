using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SendSmsCommand = SFA.DAS.Notifications.Application.Commands.SendSms.SendSmsCommand;

namespace SFA.DAS.Notifications.MessageHandlers.CommandHandlers
{
    public class SendSmsCommandHandler : IHandleMessages<Messages.Commands.SendSmsCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public SendSmsCommandHandler(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(Messages.Commands.SendSmsCommand message, IMessageHandlerContext context)
        {
            try
            {
                var command = new SendSmsCommand {
                    SystemId = "X",
                    TemplateId = message.TemplateId,
                    RecipientsNumber = message.RecipientsNumber,
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