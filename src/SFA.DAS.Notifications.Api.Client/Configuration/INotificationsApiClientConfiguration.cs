using SFA.DAS.Http;
using System;

namespace SFA.DAS.Notifications.Api.Client.Configuration
{
    public interface INotificationsApiClientConfiguration : IApiClientConfiguration
    {
        /// <summary>
        /// The base url (schema, server, port and application path as appropriate)
        /// </summary>
        /// <example>https://some-server/</example>
        string ApiBaseUrl { get; }
    }
}
