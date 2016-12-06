using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Models;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SFA.DAS.Notifications.Application.Commands.SendSms;

namespace SFA.DAS.Notifications.Api.Orchestrators
{
    public class NotificationOrchestrator : OrchestratorBase, INotificationOrchestrator
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;

        public NotificationOrchestrator(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
        }

        public async Task<OrchestratorResponse> SendEmail(Email request)
        {
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

        public async Task<OrchestratorResponse> SendSms(SendSmsRequest request)
        {
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
