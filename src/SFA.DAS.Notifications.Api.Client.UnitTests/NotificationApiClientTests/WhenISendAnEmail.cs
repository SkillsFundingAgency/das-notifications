using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Types;
using System.Net.Http;
using System;
using System.Net;

namespace SFA.DAS.Notifications.Api.Client.UnitTests.NotificationApiClientTests
{
    public class WhenISendAnEmail
    {
        private NotificationsApi _apiclient;
        private FakeResponseHandler _fakeHandler;

        private const string ExpectedApiBaseUrl = "http://test.local.url";

        [SetUp]
        public void Arrange()
        {
            _fakeHandler = new FakeResponseHandler();

            var httpClient = new HttpClient(_fakeHandler);

            _apiclient = new NotificationsApi(httpClient, new NotificationsApiClientConfiguration { ApiBaseUrl = ExpectedApiBaseUrl });
        }


        [Test]
        public async Task ThenTheApiIsCalledWithTheCorrectUrl()
        {
            var request = new TestRequest(new Uri(ExpectedApiBaseUrl + "/api/email"), JsonConvert.SerializeObject(new Email()));
            _fakeHandler.AddFakeResponse(request, new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(string.Empty) });

            await _apiclient.SendEmail(new Email());

            Assert.Pass();
        }

        [Test]
        public async Task ThenTheEmailMessageIsUsedForThePostObject()
        {
            var expectedEmailMessage = new Email
            {
                RecipientsAddress = "test@local",
                ReplyToAddress = "test2@local",
                Subject = "Test Subject",
                SystemId = "123asd",
                TemplateId = "123",
                Tokens = new Dictionary<string, string> { { "Token1","Token Value"} }
            };

            var request = new TestRequest(new Uri(ExpectedApiBaseUrl + "/api/email"), JsonConvert.SerializeObject(expectedEmailMessage));
            _fakeHandler.AddFakeResponse(request, new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(string.Empty) });

            await _apiclient.SendEmail(expectedEmailMessage);

            Assert.Pass();
        }
    }
}
