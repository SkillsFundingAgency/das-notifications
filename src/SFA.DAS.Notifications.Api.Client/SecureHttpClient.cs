using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.Notifications.Api.Client
{
    internal class SecureHttpClient
    {
        private readonly INotificationsApiClientConfiguration _configuration;

        public SecureHttpClient(INotificationsApiClientConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected SecureHttpClient()
        {
            // So we can mock for testing
        }

        private async Task<AuthenticationResult> GetAuthenticationResult(string clientId, string appKey, string resourceId, string tenant)
        {
            var authority = $"https://login.microsoftonline.com/{tenant}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(resourceId, clientCredential);
            return result;
        }

        public virtual async Task<string> GetAsync(string url)
        {
            var authenticationResult = await GetAuthenticationResult(_configuration.ClientId, _configuration.ClientSecret, _configuration.IdentifierUri, _configuration.Tenant);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public virtual async Task PostAsync(string url, Email message)
        {
            var authenticationResult = await GetAuthenticationResult(_configuration.ClientId, _configuration.ClientSecret, _configuration.IdentifierUri, _configuration.Tenant);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
                client.DefaultRequestHeaders.Add("api-version", "1");
                client.DefaultRequestHeaders.Add("accept", "application/json");

                var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
        }
    }
}