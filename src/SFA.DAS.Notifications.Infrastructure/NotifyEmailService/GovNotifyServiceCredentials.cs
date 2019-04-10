using System.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public class GovNotifyServiceCredentials
    {
        private const int ServiceIdStartPosition = 73;
        private const int ServiceApiKeyStartPosition = 36;
        private const int GuidLength = 36;

        public string ServiceId { get; }

        public string ApiKey { get; }

        public GovNotifyServiceCredentials(string serviceId, string apiKey)
        {
            ServiceId = serviceId;
            ApiKey = apiKey;
        }

        public static GovNotifyServiceCredentials FromV2ApiKey(string fromApiKey)
        {
            if (fromApiKey.Length < 74)
            {
                throw new ConfigurationErrorsException("The API Key provided is invalid. Please ensure you are using a v2 API Key that is not empty or null");
            }

            var serviceId = fromApiKey.Substring(fromApiKey.Length - ServiceIdStartPosition, GuidLength);
            var apiKey = fromApiKey.Substring(fromApiKey.Length - ServiceApiKeyStartPosition, GuidLength);

            return new GovNotifyServiceCredentials(serviceId, apiKey);
        }
    }
}
