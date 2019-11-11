using SFA.DAS.Http.Configuration;

namespace SFA.DAS.Notifications.Api.Client.Configuration
{
    public interface INotificationsApiClientConfiguration : IJwtClientConfiguration, IAzureActiveDirectoryClientConfiguration
    {
    }
}
