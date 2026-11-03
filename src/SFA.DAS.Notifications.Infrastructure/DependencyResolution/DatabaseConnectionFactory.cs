using System;
using System.Data.Common;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;

namespace SFA.DAS.Notifications.Infrastructure.DependencyResolution
{
    public static class DatabaseConnectionFactory
    {
        private const string AzureResource = "https://database.windows.net/.default";

        public static DbConnection Create(string connectionString)
        {
            var environmentName = Environment.GetEnvironmentVariable("APPSETTING_EnvironmentName");

            if (string.Equals(environmentName, "LOCAL", StringComparison.OrdinalIgnoreCase))
            {
                return new SqlConnection(connectionString);
            }

            var credential = new DefaultAzureCredential();

            return new SqlConnection
            {
                ConnectionString = connectionString,
                AccessToken = credential.GetToken(new TokenRequestContext(new[] { AzureResource })).Token
            };
        }
    }
}