using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.Notifications.Api.Client.UnitTests.NotificationApiClientTests
{
    public class WhenISendAnEmail
    {
        private Mock<SecureHttpClient> _httpClient;
        private NotificationsApi _apiclient;

        private const string ExpectedApiBaseUrl = "http://test.local.url";

        [SetUp]
        public void Arrange()
        {
            _httpClient = new Mock<SecureHttpClient>();

            _apiclient = new NotificationsApi(new NotificationsApiClientConfiguration { ApiBaseUrl = ExpectedApiBaseUrl }, _httpClient.Object);
        }


        [Test]
        public async Task ThenTheApiIsCalledWithTheCorrectUrl()
        {
            //Act
            await _apiclient.SendEmail(new Email());

            //Assert
            _httpClient.Verify(x => x.PostAsync($"{ExpectedApiBaseUrl}/api/email", It.IsAny<Email>()));
        }

        [Test]
        public async Task ThenTheEmailMessageIsUsedForThePostObject()
        {
            //Arrange
            var expectedEmailMessage = new Email
            {
                RecipientsAddress = "test@local",
                ReplyToAddress = "test2@local",
                Subject = "Test Subject",
                SystemId = "123asd",
                TemplateId = "123",
                Tokens = new Dictionary<string, string> { { "Token1","Token Value"} }
            };
            var stringContent = JsonConvert.SerializeObject(expectedEmailMessage);

            //Act
            await _apiclient.SendEmail(expectedEmailMessage);

            //Assert
            _httpClient.Verify(x => x.PostAsync(It.IsAny<string>(), It.Is<Email>(c => JsonConvert.SerializeObject(c).Equals(stringContent))));
        }
    }
}
