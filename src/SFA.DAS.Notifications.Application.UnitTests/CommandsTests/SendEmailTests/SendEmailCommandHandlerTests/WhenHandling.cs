﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Application.UnitTests.CommandsTests.SendEmailTests.SendEmailCommandHandlerTests
{
    public class WhenHandling
    {
        private const string TemplateName = "MyTemplate";
        private const string TranslatedTemplateId = "c53d62b6-df51-489b-8736-ee94d6346a28";

        private string _templateId;
        private string _systemId;
        private string _subject;
        private string _recipientsAddress;
        private string _replyToAddress;
        private Dictionary<string, string> _tokens;

        private TemplateConfiguration _templateConfiguration;
        private Mock<IEmailService> _emailService;
        private IRequestHandler<SendEmailMediatRCommand> _handler;
        private SendEmailMediatRCommand _command;

        [SetUp]
        public void Arrange()
        {
            _templateConfiguration = new TemplateConfiguration
                {
                    EmailServiceTemplates = new List<Template>
                    {
                        new Template {Id = TemplateName, EmailServiceId = TranslatedTemplateId},
                        new Template {Id = "Not" + TemplateName, EmailServiceId = "fffb72dd-ef2d-4fcd-9d41-12a23801a5ea"}
                    }
                };

            _emailService = new Mock<IEmailService>();

            _handler = new SendEmailMediatRCommandHandler(
                Mock.Of<ILogger<SendEmailMediatRCommandHandler>>(),
                _emailService.Object,
                _templateConfiguration);

            _templateId = Guid.NewGuid().ToString();
            _systemId = "Test System";
            _subject = "Test Email";
            _recipientsAddress = "testo@recipient.com";
            _replyToAddress = "replyo@recipient.com";
            _tokens = new Dictionary<string, string> {
                {"Key1", "Value1"}
            };

            _command = new SendEmailMediatRCommand
            {
                RecipientsAddress = _recipientsAddress,
                TemplateId = _templateId,
                Tokens = _tokens
            };
        }

        [Test]
        public void ThenItShouldThrowAValidationExceptionItTemplateIdNotFound()
        {
            // Arrange
            _command.TemplateId = "ThisIsNotAValidTemplateName";

            // Act + Assert
            Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(_command, new CancellationToken()));
        }

        [Test]
        public async Task ThenItShouldSendTheEmail()
        {
            // Act
            await _handler.Handle(_command, new CancellationToken());

            // Assert
            _emailService.Verify(x => x.SendAsync(
                It.Is<EmailMessage>(message =>
                    message.TemplateId == _templateId
                    && message.RecipientsAddress == _recipientsAddress
                    && message.Tokens == _tokens
                    &! string.IsNullOrEmpty(message.Reference))));
        }
    }
}
