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
        private const string AttachmentKey1 = "File1";
        private byte[] AttachmentBytes1 = new byte[10];
        private const string AttachmentKey2 = "File2";
        private byte[] AttachmentBytes2 = new byte[5];
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
                },
                Attachments = new Dictionary<string, byte[]>
                {
                    { AttachmentKey1, AttachmentBytes1 },
                    { AttachmentKey2, AttachmentBytes2 }
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

        [Test]
        public async Task ThenItShouldSendAMessageWithTheCorrectAttachments()
        {
            //Act 
            await _service.SendAsync(_email);

            //Assert
            _httpClient.Verify(c => c.SendEmail(It.Is<NotifyMessage>(m => m.Attachments != null
                                                                          && m.Attachments.ContainsKey(AttachmentKey1.ToLower())
                                                                          && m.Attachments[AttachmentKey1.ToLower()] == AttachmentBytes1
                                                                          && m.Attachments.ContainsKey(AttachmentKey2.ToLower())
                                                                          && m.Attachments[AttachmentKey2.ToLower()] == AttachmentBytes2)));
        }

        [Test]
        public async Task ThenItShouldSendMessageWithNoAttachmentIfAttachmentsIsNull()
        {
            //Arrange
            _email.Attachments = null;

            //Act
            await _service.SendAsync(_email);

            //Assert
            _httpClient.Verify(c => c.SendEmail(It.Is<NotifyMessage>(m => m.Attachments != null)));
        }
    }
}