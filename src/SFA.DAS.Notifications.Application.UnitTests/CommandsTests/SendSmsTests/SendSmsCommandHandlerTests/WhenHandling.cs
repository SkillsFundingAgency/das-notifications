using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Application.Commands.SendSms;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Repositories;

namespace SFA.DAS.Notifications.Application.UnitTests.CommandsTests.SendSmsTests.SendSmsCommandHandlerTests
{
    public class WhenHandling
    {
        private const string TemplateName = "MyTemplate";
        private const string TranslatedTemplateId = "c53d62b6-df51-489b-8736-ee94d6346a28";

        private Mock<INotificationsRepository> _notificationsRepository;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<ITemplateConfigurationService> _templateConfigurationService;
        private SendSmsCommandHandler _handler;
        private SendSmsCommand _command;

        [SetUp]
        public void Arrange()
        {
            _notificationsRepository = new Mock<INotificationsRepository>();

            _messagePublisher = new Mock<IMessagePublisher>();

            _templateConfigurationService = new Mock<ITemplateConfigurationService>();
            _templateConfigurationService.Setup(s => s.GetAsync())
                .ReturnsAsync(new TemplateConfiguration
                {
                    SmsServiceTemplates = new List<SmsTemplate>
                    {
                        new SmsTemplate {Id = TemplateName, ServiceId = TranslatedTemplateId},
                        new SmsTemplate {Id = "Not" + TemplateName, ServiceId = "fffb72dd-ef2d-4fcd-9d41-12a23801a5ea"}
                    }
                });

            _handler = new SendSmsCommandHandler(
                _notificationsRepository.Object,
                _messagePublisher.Object,
                _templateConfigurationService.Object,
                Mock.Of<ILog>());

            _command = new SendSmsCommand
            {
                SystemId = Guid.NewGuid().ToString(),
                RecipientsNumber = "999",
                TemplateId = TemplateName,
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
            var expectedData = JsonConvert.SerializeObject(new NotificationSmsContent
            {
                RecipientsNumber = _command.RecipientsNumber,
                Tokens = _command.Tokens
            });
            _notificationsRepository.Verify(r => r.Create(
                It.Is<Notification>(n => 
                    n.Status == NotificationStatus.New
                    && n.Format == NotificationFormat.Sms
                    && n.TemplateId == _command.TemplateId
                    && n.Data == expectedData)),
                Times.Once);
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
