using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using NLog;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Models;
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

        public async Task<OrchestratorResponse> SendEmail(SendEmailRequest request)
        {
            try
            {
                var command = new SendEmailCommand
                {
                    SystemId = request.SystemId,
                    TemplateId = request.TemplateId,
                    Subject = request.Subject,
                    RecipientsAddress = request.RecipientsAddress,
                    ReplyToAddress = request.ReplyToAddress,
                    Tokens = request.Tokens
                };

                await _mediator.SendAsync(command);

                return GetOrchestratorResponse(NotificationOrchestratorCodes.Post.Success);
            }
            catch (ValidationException ex)
            {
                //todo: this is not logging to eventhub
                Logger.Warn(ex, "Invalid request");
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw;
            }
        }

        public async Task<OrchestratorResponse> SendSms(SendSmsRequest request)
        {
            try
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
            catch (ValidationException ex)
            {
                //todo: this is not logging to eventhub
                Logger.Warn(ex, "Invalid request");
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw;
            }
        }
    }
}
