using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SFA.DAS.Notifications.Messages.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Notifications.MessageHandlers.CommandHandlers
{
    public class SendEmailWithAttachmentsCommandHandler : IHandleMessages<SendEmailWithAttachmentsCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SendEmailWithAttachmentsCommandHandler> _logger;

        public SendEmailWithAttachmentsCommandHandler(IMediator mediator, ILogger<SendEmailWithAttachmentsCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(SendEmailWithAttachmentsCommand message, IMessageHandlerContext context)
        {
            try
            {
                var command = new SendEmailMediatRCommand
                {
                    TemplateId = message.TemplateId,
                    RecipientsAddress = message.RecipientsAddress,
                    Tokens = message.Tokens.ToDictionary(e => e.Key, e => e.Value),
                    Attachments = message.Attachments.Value
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