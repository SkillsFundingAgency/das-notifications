using System;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public class GovNotifyServiceCredentials
    {
        private const int ServiceIdStartPosition = 73;
        private const int ServiceApiKeyStartPosition = 36;
        private const int GuidLength = 36;

        public string ServiceId { get; }
        public string ApiKey { get; }

        public GovNotifyServiceCredentials(string apiKey)
        {
            if (apiKey.Length < 74)
            {
                throw new Exception("The API Key provided is invalid. Please ensure you are using a v2 API Key that is not empty or null");
            }

            ServiceId = apiKey.Substring(apiKey.Length - ServiceIdStartPosition, GuidLength);
            ApiKey = apiKey.Substring(apiKey.Length - ServiceApiKeyStartPosition, GuidLength);
        }
    }
}
