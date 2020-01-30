using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Infrastructure.ExecutionPolicies;
using SFA.DAS.Notifications.Infrastructure.NotifyEmailService;

namespace SFA.DAS.Notifications.Infrastructure.UnitTests.NotifyEmailServiceTests
{
    public class WhenSendingAnSms
    {
        private const string ToNumber = "299792458";
        private const string TemplateId = "81476cee-9863-40ee-8b18-33dcb898e9c9";
        private const string TokenKey = "Key1";
        private const string TokenValue = "Value1";
        private const string SystemId = "TestSystem";

        private Mock<INotifyClientWrapper> _httpClient;
        private NotifyEmailService.NotifySmsService _service;
        private SmsMessage _sms;
        private NoopExecutionPolicy _executionPolicy;

        [SetUp]
        public void Arrange()
        {
            _httpClient = new Mock<INotifyClientWrapper>();

            _executionPolicy = new NoopExecutionPolicy();

            _service = new NotifyEmailService.NotifySmsService(_httpClient.Object, _executionPolicy);

            _sms = new SmsMessage
            {
                RecipientsNumber = ToNumber,
                TemplateId = TemplateId,
                Tokens = new Dictionary<string, string>
                {
                    { TokenKey, TokenValue }
                },
                SystemId = SystemId
            };
        }

        [Test]
        public async Task ThenItShouldSendAMessageToTheCorrectReceipient()
        {
            // Act
            await _service.SendAsync(_sms);

            // Assert
            _httpClient.Verify(c => c.SendSms(It.Is<NotifyMessage>(m => m.To == ToNumber)));
        }

        [Test]
        public async Task ThenItShouldSendAMessageWithTheCorrectTemplate()
        {
            // Act
            await _service.SendAsync(_sms);

            // Assert
            _httpClient.Verify(c => c.SendSms(It.Is<NotifyMessage>(m => m.Template == TemplateId)));
        }

        [Test]
        public async Task ThenItShouldSendAMessageWithTheCorrectPersonalisation()
        {
            // Act
            await _service.SendAsync(_sms);

            // Assert
            _httpClient.Verify(c => c.SendSms(It.Is<NotifyMessage>(m => m.Personalisation != null
                                                                         && m.Personalisation.ContainsKey(TokenKey.ToLower())
                                                                         && m.Personalisation[TokenKey.ToLower()] == TokenValue)));
        }

        [Test]
        public async Task ThenItShouldSendMessageWithNoPersonalisationIfTokensIsNull()
        {
            // Arrange
            _sms.Tokens = null;

            // Act
            await _service.SendAsync(_sms);

            // Assert
            _httpClient.Verify(c => c.SendSms(It.Is<NotifyMessage>(m => m.Personalisation != null)));
        }
    }
}
