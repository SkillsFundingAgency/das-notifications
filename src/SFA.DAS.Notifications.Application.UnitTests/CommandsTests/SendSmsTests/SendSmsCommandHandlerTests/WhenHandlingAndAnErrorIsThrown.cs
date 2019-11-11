using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Moq;
using NUnit.Framework;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Application.Commands.SendSms;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Repositories;

namespace SFA.DAS.Notifications.Application.UnitTests.CommandsTests.SendSmsTests.SendSmsCommandHandlerTests
{
    public class WhenHandlingAndAnErrorIsThrown
    {
        private const string TemplateName = "MyTemplate";
        private const string TranslatedTemplateId = "c53d62b6-df51-489b-8736-ee94d6346a28";

        private string _systemId;
        private string _recipientsNumber;
        private Dictionary<string, string> _tokens;

        private Mock<INotificationsRepository> _notificationsRepository;
        private Mock<ITemplateConfigurationService> _templateConfigurationService;
        private Mock<ISmsService> _smsService;
        private SendSmsCommandHandler _handler;
        private SendSmsCommand _command;

        [SetUp]
        public void Arrange()
        {
            _notificationsRepository = new Mock<INotificationsRepository>();

            _templateConfigurationService = new Mock<ITemplateConfigurationService>();
            _templateConfigurationService.Setup(s => s.GetAsync())
                .ReturnsAsync(new TemplateConfiguration {
                    SmsServiceTemplates = new List<SmsTemplate>
                    {
                        new SmsTemplate {Id = TemplateName, ServiceId = TranslatedTemplateId},
                        new SmsTemplate {Id = "Not" + TemplateName, ServiceId = "fffb72dd-ef2d-4fcd-9d41-12a23801a5ea"}
                    }
                });

            _smsService = new Mock<ISmsService>();

            _smsService.Setup(x => x.SendAsync(It.IsAny<SmsMessage>())).Throws(new Exception());

            _handler = new SendSmsCommandHandler(
                _notificationsRepository.Object,
                _templateConfigurationService.Object,
                Mock.Of<ILog>(),
                _smsService.Object);

            _systemId = "Test System";
            _recipientsNumber = "07123456789";
            _tokens = new Dictionary<string, string> {
                {"Key1", "Value1"}
            };


            _command = new SendSmsCommand {
                SystemId = _systemId,
                RecipientsNumber = _recipientsNumber,
                TemplateId = TemplateName,
                Tokens = _tokens
            };
        }

        [Test]
        public async Task ThenItShouldMarkTheSMSAsFailed()
        {
            // Act
            try
            {
                await _handler.Handle(_command);
            }
            catch{}
            finally
            {
                // Assert
                _notificationsRepository.Verify(r => r.Update(
                        NotificationFormat.Sms, It.Is<string>(x => !x.IsNullOrEmpty()), NotificationStatus.Failed),
                    Times.Once);
            }
        }
    }
}