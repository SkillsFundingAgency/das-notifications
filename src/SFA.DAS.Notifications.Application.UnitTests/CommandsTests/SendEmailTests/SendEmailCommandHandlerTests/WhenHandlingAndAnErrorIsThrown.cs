using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Moq;
using NUnit.Framework;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Repositories;

namespace SFA.DAS.Notifications.Application.UnitTests.CommandsTests.SendEmailTests.SendEmailCommandHandlerTests
{
    public class WhenHandlingAndAnErrorIsThrown
    {
        private const string TemplateName = "MyTemplate";
        private const string TranslatedTemplateId = "c53d62b6-df51-489b-8736-ee94d6346a28";

        private string _templateId;
        private string _systemId;
        private string _subject;
        private string _recipientsAddress;
        private string _replyToAddress;
        private Dictionary<string, string> _tokens;

        private Mock<INotificationsRepository> _notificationsRepository;
        private Mock<ITemplateConfigurationService> _templateConfigurationService;
        private Mock<IEmailService> _emailService;
        private SendEmailCommandHandler _handler;
        private SendEmailCommand _command;

        [SetUp]
        public void Arrange()
        {
            _notificationsRepository = new Mock<INotificationsRepository>();

            _templateConfigurationService = new Mock<ITemplateConfigurationService>();
            _templateConfigurationService.Setup(s => s.GetAsync())
                .ReturnsAsync(new TemplateConfiguration {
                    EmailServiceTemplates = new List<Template>
                    {
                        new Template {Id = TemplateName, EmailServiceId = TranslatedTemplateId},
                        new Template {Id = "Not" + TemplateName, EmailServiceId = "fffb72dd-ef2d-4fcd-9d41-12a23801a5ea"}
                    }
                });

            _emailService = new Mock<IEmailService>();

            _handler = new SendEmailCommandHandler(
                _notificationsRepository.Object,
                _templateConfigurationService.Object,
                Mock.Of<ILog>(),
                _emailService.Object);

            _templateId = Guid.NewGuid().ToString();
            _systemId = "Test System";
            _subject = "Test Email";
            _recipientsAddress = "testo@recipient.com";
            _replyToAddress = "replyo@recipient.com";
            _tokens = new Dictionary<string, string> {
                {"Key1", "Value1"}
            };

            _command = new SendEmailCommand {
                SystemId = _systemId,
                Subject = _subject,
                RecipientsAddress = _recipientsAddress,
                ReplyToAddress = _replyToAddress,
                TemplateId = _templateId,
                Tokens = _tokens
            };

            _emailService.Setup(x => x.SendAsync(It.IsAny<EmailMessage>())).Throws(new Exception());
        }

        [Test]
        public async Task ThenItShouldMarkTheEmailAsFailed()
        {
            // Act
            await _handler.Handle(_command);

            // Assert
            _notificationsRepository.Verify(r => r.Update(
                    NotificationFormat.Email, It.Is<string>(x => !x.IsNullOrEmpty()), NotificationStatus.Failed),
                Times.Once);
        }
    }
}