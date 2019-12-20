using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
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

        private TemplateConfiguration _templateConfiguration;
        private Mock<ISmsService> _smsService;
        private IRequestHandler<SendSmsCommand> _handler;
        private SendSmsCommand _command;

        [SetUp]
        public void Arrange()
        {
            _templateConfiguration = new TemplateConfiguration
            {
                SmsServiceTemplates = new List<SmsTemplate>
                    {
                        new SmsTemplate {Id = TemplateName, ServiceId = TranslatedTemplateId},
                        new SmsTemplate {Id = "Not" + TemplateName, ServiceId = "fffb72dd-ef2d-4fcd-9d41-12a23801a5ea"}
                    }
            };

            _smsService = new Mock<ISmsService>();

            _handler = new SendSmsCommandHandler(
                _templateConfiguration,
                Mock.Of<ILogger>(),
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
            Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(_command, new CancellationToken()));
        }

        [Test]
        public async Task ThenItShouldSendTheSms()
        {
            // Act
            await _handler.Handle(_command, new CancellationToken());

            // Assert
            _smsService.Verify(x => x.SendAsync(
                It.Is<SmsMessage>(message =>
                    message.TemplateId == _templateId
                    && message.SystemId == _systemId
                    && message.RecipientsNumber == _recipientsNumber
                    && message.Tokens == _tokens
                    & !string.IsNullOrEmpty(message.Reference))));
        }
    }
}
