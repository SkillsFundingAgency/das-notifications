﻿using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Application.Queries.GetMessage;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Application.Commands.DispatchNotification
{
    public class DispatchNotificationCommandHandler : AsyncRequestHandler<DispatchNotificationCommand>
    {
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public DispatchNotificationCommandHandler(IMediator mediator, IEmailService emailService, ISmsService smsService)
        {
            _mediator = mediator;
            _emailService = emailService;
            _smsService = smsService;
        }

        protected override async Task HandleCore(DispatchNotificationCommand command)
        {
            var response = await _mediator.SendAsync(new GetMessageQueryRequest
            {
                MessageId = command.MessageId
            });

            var notificationFormat = response.Notification.Format;

            switch (notificationFormat)
            {
                case NotificationFormat.Email:
                    //todo: log here
                    var emailContent = JsonConvert.DeserializeObject<NotificationEmailContent>(response.Notification.Data);

                    await _emailService.SendAsync(new EmailMessage
                    {
                        TemplateId = response.Notification.TemplateId,
                        SystemId = response.Notification.SystemId,
                        Subject = emailContent.Subject,
                        RecipientsAddress = emailContent.RecipientsAddress,
                        ReplyToAddress = emailContent.ReplyToAddress,
                        Tokens = emailContent.Tokens
                    });
                    break;

                case NotificationFormat.Sms:
                    var smsContent = JsonConvert.DeserializeObject<NotificationSmsContent>(response.Notification.Data);
                    throw new NotImplementedException();

                default:
                    throw new ArgumentOutOfRangeException(nameof(notificationFormat), "Unsupported notification format");
            }
        }
    }
}