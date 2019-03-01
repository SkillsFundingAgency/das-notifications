using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SFA.DAS.Notifications.Application.Commands.SendSms;

namespace SFA.DAS.Notifications.Api.Orchestrators
{
    public class NotificationOrchestrator : OrchestratorBase, INotificationOrchestrator
    {
        private readonly ILog _logger;
        private readonly IMediator _mediator;

        public NotificationOrchestrator(IMediator mediator, ILog logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _logger = logger;
        }

        public async Task<OrchestratorResponse> SendEmail(Email request)
        {
            _logger.Info($"Received request to send email to {request.RecipientsAddress}");

            await _mediator.SendAsync(new SendEmailCommand
            {
                SystemId = request.SystemId,
                TemplateId = request.TemplateId,
                Subject = request.Subject,
                RecipientsAddress = request.RecipientsAddress,
                ReplyToAddress = request.ReplyToAddress,
                Tokens = request.Tokens
            });

            return GetOrchestratorResponse(NotificationOrchestratorCodes.Post.Success);
        }

        public async Task<OrchestratorResponse> SendSms(Sms request)
        {
            _logger.Info($"Received request to send sms to {request.RecipientsNumber}");

            var command = new SendSmsCommand
            {
                SystemId = request.SystemId,
                TemplateId = request.TemplateId,
                RecipientsNumber = request.RecipientsNumber,
                Tokens = request.Tokens
            };

            await _mediator.SendAsync(command);

            return GetOrchestratorResponse(NotificationOrchestratorCodes.Post.Success);
        }
    }
}
