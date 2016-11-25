using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Application.Queries.GetMessage;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Repositories;

namespace SFA.DAS.Notifications.Application.Commands.DispatchNotification
{
    public class DispatchNotificationCommandHandler : AsyncRequestHandler<DispatchNotificationCommand>
    {
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly INotificationsRepository _notificationsRepository;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public DispatchNotificationCommandHandler(IMediator mediator, IEmailService emailService, ISmsService smsService, INotificationsRepository notificationsRepository)
        {
            _mediator = mediator;
            _emailService = emailService;
            _smsService = smsService;
            _notificationsRepository = notificationsRepository;
        }

        protected override async Task HandleCore(DispatchNotificationCommand command)
        {
            var response = await _mediator.SendAsync(new GetMessageQueryRequest
            {
                Format = command.Format,
                MessageId = command.MessageId
            });

            var notificationFormat = response.Notification.Format;

            switch (notificationFormat)
            {
                case NotificationFormat.Email:
                    Logger.Info($"Handling dispatch email message {command.MessageId}");

                    try
                    {
                        var emailContent = JsonConvert.DeserializeObject<NotificationEmailContent>(response.Notification.Data);

                        await _notificationsRepository.Update(command.Format, command.MessageId, NotificationStatus.Sending);

                        await _emailService.SendAsync(new EmailMessage
                        {
                            TemplateId = response.Notification.TemplateId,
                            SystemId = response.Notification.SystemId,
                            Subject = emailContent.Subject,
                            RecipientsAddress = emailContent.RecipientsAddress,
                            ReplyToAddress = emailContent.ReplyToAddress,
                            Tokens = emailContent.Tokens
                        });

                        await _notificationsRepository.Update(command.Format, command.MessageId, NotificationStatus.Sent);
                    }
                    catch (Exception)
                    {
                        await _notificationsRepository.Update(command.Format, command.MessageId, NotificationStatus.Failed);

                        throw;
                    }

                    break;

                case NotificationFormat.Sms:
                    Logger.Info($"Handling dispatch SMS message {command.MessageId}");

                    try
                    {
                        var smsContent = JsonConvert.DeserializeObject<NotificationSmsContent>(response.Notification.Data);

                        await _notificationsRepository.Update(command.Format, command.MessageId, NotificationStatus.Sending);

                        await _smsService.SendAsync(new SmsMessage
                        {
                            TemplateId = response.Notification.TemplateId,
                            SystemId = response.Notification.SystemId,
                            RecipientsNumber = smsContent.RecipientsNumber,
                            Tokens = smsContent.Tokens
                        });

                        await _notificationsRepository.Update(command.Format, command.MessageId, NotificationStatus.Sent);
                    }
                    catch (Exception)
                    {
                        await _notificationsRepository.Update(command.Format, command.MessageId, NotificationStatus.Failed);

                        throw;
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(notificationFormat), "Unsupported notification format");
            }
        }
    }
}
