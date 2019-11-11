using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using FluentValidation;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Application.Commands.SendSms;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Application.UnitTests.CommandsTests.SendSmsTests.SendSmsCommandHandlerTests
{
    public class WhenHandling
    {
        private const string TemplateName = "MyTemplate";
        private const string TranslatedTemplateId = "c53d62b6-df51-489b-8736-ee94d6346a28";

        private string _templateId;
        private string _systemId;
        private string _recipientsNumber;
        private Dictionary<string, string> _tokens;

        private Mock<ITemplateConfigurationService> _templateConfigurationService;
        private Mock<ISmsService> _smsService;
        private SendSmsCommandHandler _handler;
        private SendSmsCommand _command;

        [SetUp]
        public void Arrange()
        {
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

            _smsService = new Mock<ISmsService>();

            _handler = new SendSmsCommandHandler(
                _templateConfigurationService.Object,
                Mock.Of<ILog>(),
                _smsService.Object);

            _templateId = TranslatedTemplateId;
            _systemId = "Test System";
            _recipientsNumber = "07123456789";
            _tokens = new Dictionary<string, string> {
                {"Key1", "Value1"}
            };


            _command = new SendSmsCommand
            {
                SystemId = _systemId,
                RecipientsNumber = _recipientsNumber,
                TemplateId = TemplateName,
                Tokens = _tokens
            };
        }

        [Test]
        public void ThenItShouldThrowAValidationExceptionIfTemplateIdNotFound()
        {
            // Arrange
            _command.TemplateId = "ThisIsNotAValidTemplateName";

            // Act + Assert
            Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public async Task ThenItShouldSendTheSms()
        {
            // Act
            await _handler.Handle(_command);

            // Assert
            _smsService.Verify(x => x.SendAsync(
                It.Is<SmsMessage>(message =>
                    message.TemplateId == _templateId
                    && message.SystemId == _systemId
                    && message.RecipientsNumber == _recipientsNumber
                    && message.Tokens == _tokens
                    & !message.Reference.IsNullOrEmpty())));
        }
    }
}
