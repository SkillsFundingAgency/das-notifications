using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Infrastructure.ExecutionPolicies;
using SFA.DAS.Notifications.Infrastructure.NotifyEmailService;

namespace SFA.DAS.Notifications.Infrastructure.UnitTests.NotifyEmailServiceTests
{
    public class WhenSendingAnEmail
    {
        private const string ToAddress = "user.one@unit.test";
        private const string TemplateId = "81476cee-9863-40ee-8b18-33dcb898e9c9";
        private const string TokenKey = "Key1";
        private const string TokenValue = "Value1";
        private const string SystemId = "TestSystem";

        private Mock<INotifyClientWrapper> _httpClient;
        private NotifyEmailService.NotifyEmailService _service;
        private EmailMessage _email;
        private NoopExecutionPolicy _executionPolicy;

        [SetUp]
        public void Arrange()
        {
            _httpClient = new Mock<INotifyClientWrapper>();

            _executionPolicy = new NoopExecutionPolicy();

            _service = new NotifyEmailService.NotifyEmailService(_httpClient.Object, _executionPolicy);

            _email = new EmailMessage
            {
                RecipientsAddress = ToAddress,
                TemplateId = TemplateId,
                Tokens = new Dictionary<string, string>
                {
                    { TokenKey, TokenValue }
                }
            };
        }

        [Test]
        public async Task ThenItShouldSendAMessageToTheCorrectReceipient()
        {
            // Act
            await _service.SendAsync(_email);

            // Assert
            _httpClient.Verify(c => c.SendEmail(It.Is<NotifyMessage>(m => m.To == ToAddress)));
        }

        [Test]
        public async Task ThenItShouldSendAMessageWithTheCorrectTemplate()
        {
            // Act
            await _service.SendAsync(_email);

            // Assert
            _httpClient.Verify(c => c.SendEmail(It.Is<NotifyMessage>(m => m.Template == TemplateId)));
        }

        [Test]
        public async Task ThenItShouldSendAMessageWithTheCorrectPersonalisation()
        {
            // Act
            await _service.SendAsync(_email);

            // Assert
            _httpClient.Verify(c => c.SendEmail(It.Is<NotifyMessage>(m => m.Personalisation != null
                                                                         && m.Personalisation.ContainsKey(TokenKey.ToLower())
                                                                         && m.Personalisation[TokenKey.ToLower()] == TokenValue)));
        }

        [Test]
        public async Task ThenItShouldSendMessageWithNoPersonalisationIfTokensIsNull()
        {
            // Arrange
            _email.Tokens = null;

            // Act
            await _service.SendAsync(_email);

            // Assert
            _httpClient.Verify(c => c.SendEmail(It.Is<NotifyMessage>(m => m.Personalisation != null)));
        }
    }
}
