using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public static class ConfigurationExtensions
    {
        private static string ConfigurationKey = "SFA.DAS.Notifications";
        public static IConfigurationSection GetNotificationSection(this IConfiguration configuration, params string[] subSectionPaths)
        {
            var key = string.Join(":",
                Enumerable.Repeat(ConfigurationKey, 1)
                    .Concat(subSectionPaths));
            return configuration.GetSection(key);
        }

        public static TConfiguration GetNotificationSection<TConfiguration>(this IConfiguration configuration, params string[] subSectionPaths)
        {
            return configuration.GetNotificationSection(subSectionPaths).Get<TConfiguration>();
        }
    }
}
