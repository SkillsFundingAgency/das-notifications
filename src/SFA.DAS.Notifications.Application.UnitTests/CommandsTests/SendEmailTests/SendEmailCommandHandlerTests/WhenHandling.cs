using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Repositories;

namespace SFA.DAS.Notifications.Application.UnitTests.CommandsTests.SendEmailTests.SendEmailCommandHandlerTests
{
    public class WhenHandling
    {
        private const string TemplateName = "MyTemplate";
        private const string TranslatedTemplateId = "c53d62b6-df51-489b-8736-ee94d6346a28";

        private Mock<INotificationsRepository> _notificationsRepository;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<ITemplateConfigurationService> _templateConfigurationService;
        private SendEmailCommandHandler _handler;
        private SendEmailCommand _command;

        [SetUp]
        public void Arrange()
        {
            _notificationsRepository = new Mock<INotificationsRepository>();

            _messagePublisher = new Mock<IMessagePublisher>();

            _templateConfigurationService = new Mock<ITemplateConfigurationService>();
            _templateConfigurationService.Setup(s => s.GetAsync())
                .ReturnsAsync(new TemplateConfiguration
                {
                    EmailServiceTemplates = new System.Collections.Generic.List<Template>
                    {
                        new Template {Id = TemplateName, EmailServiceId = TranslatedTemplateId},
                        new Template {Id = "Not" + TemplateName, EmailServiceId = "fffb72dd-ef2d-4fcd-9d41-12a23801a5ea"}
                    }
                });

            _handler = new SendEmailCommandHandler(
                _notificationsRepository.Object,
                _messagePublisher.Object,
                _templateConfigurationService.Object,
                Mock.Of<ILog>());

            _command = new SendEmailCommand
            {
                SystemId = Guid.NewGuid().ToString(),
                Subject = "Unit test emails",
                RecipientsAddress = "user.one@unit.tests",
                ReplyToAddress = "noreply@unit.tests",
                TemplateId = Guid.NewGuid().ToString(),
                Tokens = new Dictionary<string, string>
                {
                    {"Key1", "Value1"}
                }
            };
        }

        [Test]
        public async Task ThenItShouldSaveTheMessageToTheRepository()
        {
            // Act
            await _handler.Handle(_command);

            // Assert
            var expectedData = JsonConvert.SerializeObject(new NotificationEmailContent
            {
                Subject = _command.Subject,
                RecipientsAddress = _command.RecipientsAddress,
                ReplyToAddress = _command.ReplyToAddress,
                Tokens = _command.Tokens
            });
            _notificationsRepository.Verify(r => r.Create(It.Is<Notification>(n => n.Status == NotificationStatus.New)), Times.Once);
            _notificationsRepository.Verify(r => r.Create(It.Is<Notification>(n => n.Format == NotificationFormat.Email)), Times.Once);
            _notificationsRepository.Verify(r => r.Create(It.Is<Notification>(n => n.TemplateId == _command.TemplateId)), Times.Once);
            _notificationsRepository.Verify(r => r.Create(It.Is<Notification>(n => n.Data == expectedData)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldTranslateTemplateIdFromServiceForRequestedTemplateIfNotAGuid()
        {
            // Arrange
            _command.TemplateId = TemplateName;

            // Act
            await _handler.Handle(_command);

            // Assert
            _notificationsRepository.Verify(r => r.Create(It.Is<Notification>(n => n.TemplateId == TranslatedTemplateId)), Times.Once);
        }

        [Test]
        public void ThenItShouldThrowAValidationExceptionItTemplateIdNotFound()
        {
            // Arrange
            _command.TemplateId = "ThisIsNotAValidTemplateName";

            // Act + Assert
            Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(_command));
        }
    }
}
