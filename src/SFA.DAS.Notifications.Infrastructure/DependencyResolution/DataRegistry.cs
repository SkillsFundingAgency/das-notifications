using System;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Notifications.Infrastructure.DependencyResolution
{
    public class DataRegistry : Registry
    {
        private const string AzureResource = "https://database.windows.net/";

        public DataRegistry()
        {
            For<DbConnection>().Use($"Build DbConnection", c =>
            {
                var environmentName = Environment.GetEnvironmentVariable("APPSETTING_EnvironmentName");
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                return environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase)
                    ? new SqlConnection(c.GetInstance<NotificationServiceConfiguration>().DatabaseConnectionString)
                    : new SqlConnection
                    {
                        ConnectionString = c.GetInstance<NotificationServiceConfiguration>().DatabaseConnectionString,
                        AccessToken = azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result
                    };
            });
        }
    }

}