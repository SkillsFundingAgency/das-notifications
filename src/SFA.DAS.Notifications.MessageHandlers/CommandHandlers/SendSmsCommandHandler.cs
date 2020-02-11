using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Notifications.Application.Commands.SendSms;

namespace SFA.DAS.Notifications.MessageHandlers.CommandHandlers
{
    public class SendSmsCommandHandler : IHandleMessages<Messages.Commands.SendSmsCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SendSmsCommandHandler> _logger;

        public SendSmsCommandHandler(IMediator mediator, ILogger<SendSmsCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(Messages.Commands.SendSmsCommand message, IMessageHandlerContext context)
        {
            try
            {
                var command = new SendSmsMediatRCommand {
                    SystemId = "X",
                    TemplateId = message.TemplateId,
                    RecipientsNumber = message.RecipientsNumber,
                    Tokens = message.Tokens.ToDictionary(e => e.Key, e => e.Value)
                };

                await _mediator.Send(command);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }

        }
    }
}