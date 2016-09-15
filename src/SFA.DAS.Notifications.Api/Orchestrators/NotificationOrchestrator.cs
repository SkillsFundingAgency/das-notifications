using System;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using NLog;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Models;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SFA.DAS.Notifications.Application.Commands.SendSms;
using SFA.DAS.Notifications.Application.Exceptions;

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

                var validationResult = ValidateCommand(command);

                if (!validationResult.IsValid)
                    return GetOrchestratorResponse(NotificationOrchestratorCodes.Post.ValidationFailure, validationResult: validationResult);

                await _mediator.SendAsync(command);

                return GetOrchestratorResponse(NotificationOrchestratorCodes.Post.Success);
            }
            catch (CustomValidationException ex)
            {
                Logger.Info($"Validation error {ex.Message}");
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

                var validationResult = ValidateCommand(command);

                if (!validationResult.IsValid)
                    return GetOrchestratorResponse(NotificationOrchestratorCodes.Post.ValidationFailure, validationResult: validationResult);

                await _mediator.SendAsync(command);

                return GetOrchestratorResponse(NotificationOrchestratorCodes.Post.Success);
            }
            catch (CustomValidationException ex)
            {
                Logger.Info($"Validation error {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw;
            }
        }

        private static ValidationResult ValidateCommand(SendEmailCommand command)
        {
            var validator = new SendEmailCommandValidator();

            return validator.Validate(command);
        }

        private static ValidationResult ValidateCommand(SendSmsCommand command)
        {
            var validator = new SendSmsCommandValidator();

            return validator.Validate(command);
        }
    }
}
