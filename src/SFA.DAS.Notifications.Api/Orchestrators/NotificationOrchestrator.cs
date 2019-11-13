using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Core;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.Notifications.Api.Orchestrators
{
    public class NotificationOrchestrator : OrchestratorBase, INotificationOrchestrator
    {
        private readonly ILog _logger;
        private readonly IMessageSession _publisher;

        public NotificationOrchestrator(ILog logger, IMessageSession publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }

        public async Task<OrchestratorResponse> SendEmail(Email request)
        {
            _logger.Info($"Received request to send email to {request.RecipientsAddress}");

            await _publisher.Send(new SendEmailCommand(
                request.TemplateId,
                request.RecipientsAddress,
                request.ReplyToAddress,
                request.Tokens
            ));

            return GetOrchestratorResponse(NotificationOrchestratorCodes.Post.Success);
        }

        public async Task<OrchestratorResponse> SendSms(Sms request)
        {
            _logger.Info($"Received request to send sms to {request.RecipientsNumber}");

            await _publisher.Send(new SendSmsCommand(
                request.SystemId,
                request.TemplateId,
                request.RecipientsNumber,
                request.Tokens
            ));

            return GetOrchestratorResponse(NotificationOrchestratorCodes.Post.Success);
        }
    }
}
