using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NServiceBus;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;

namespace SFA.DAS.Notifications.MessageHandlers.TestHarness
{
    internal class Program
    {
        public static async Task Main()
        {
            var builder = new ConfigurationBuilder()
                .AddAzureTableStorage("SFA.DAS.Notifications.MessageHandlers");

           IConfigurationRoot configuration = builder.Build();

            var provider = new ServiceCollection()
                .AddOptions()
                .Configure<NotificationServiceConfiguration>(configuration.GetSection("SFA.DAS.Notifications.MessageHandlers")).BuildServiceProvider();

            var config = provider.GetService<IOptions<NotificationServiceConfiguration>>().Value.NServiceBusConfiguration;
            var isDevelopment = Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName) == "LOCAL";

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Notifications.MessageHandlers.TestHarness")
                .UseErrorQueue()
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            if (isDevelopment)
            {
                endpointConfiguration.UseLearningTransport(s => s.AddRouting());
            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(config.ServiceBusConnectionString, s => s.AddRouting());
            }

            var blobServiceClient = new BlobServiceClient("UseDevelopmentStorage=true");
            var dataBus = endpointConfiguration.UseDataBus<AzureDataBus>()
                        .Container("testcontainer")
                        .UseBlobServiceClient(blobServiceClient);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningDataBusPropertiesAs(property =>
            {
                return property.Name.EndsWith("DataBus");
            });

            var endpoint = await Endpoint.Start(endpointConfiguration);

            var testHarness = new TestHarness(endpoint);

            await testHarness.Run();
            await endpoint.Stop();
        }
    }

    class NotificationServiceConfiguration
    {
        public NServiceBusConfiguration NServiceBusConfiguration { get; set; }
    }

    class NServiceBusConfiguration
    {
        public string ServiceBusConnectionString { get; set; }
        public string NServiceBusLicense { get; set; }
    }
}