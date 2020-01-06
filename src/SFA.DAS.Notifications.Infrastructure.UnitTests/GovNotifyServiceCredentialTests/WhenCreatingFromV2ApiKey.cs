using NUnit.Framework;
using SFA.DAS.Notifications.Infrastructure.NotifyEmailService;

namespace SFA.DAS.Notifications.Infrastructure.UnitTests.GovNotifyServiceCredentialTests
{
    public class WhenCreatingFromV2ApiKey
    {
        private const string V2ApiKey =
            "samplekey-11111111-1111-1111-1111-111111111111-22222222-2222-2222-2222-222222222222";
        private const string ServiceId = "11111111-1111-1111-1111-111111111111";
        private const string ApiKey = "22222222-2222-2222-2222-222222222222";

        private GovNotifyServiceCredentials _serviceCredentials;

        [SetUp]
        public void Arrange()
        {
            _serviceCredentials = new GovNotifyServiceCredentials(V2ApiKey);
        }

        [Test]
        public void ThenServiceIdShouldBeSet()
        {
            Assert.AreEqual(ServiceId, _serviceCredentials.ServiceId);
        }

        [Test]
        public void ThenApiKeyShouldBeSet()
        {
            Assert.AreEqual(ApiKey, _serviceCredentials.ApiKey);
        }
    }
}
