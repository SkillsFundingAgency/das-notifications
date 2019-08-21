using System;

namespace SFA.DAS.Notifications.Api.Client.Configuration
{
    public class NotificationsApiClientConfiguration : INotificationsApiClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        
        [Obsolete("Azure AD authentication is obsolete. Use JWT authentication.")]
        public string ClientId { get; set; }
        [Obsolete("Azure AD authentication is obsolete. Use JWT authentication.")]
        public string ClientSecret { get; set; }
        [Obsolete("Azure AD authentication is obsolete. Use JWT authentication.")]
        public string IdentifierUri { get; set; }
        [Obsolete("Azure AD authentication is obsolete. Use JWT authentication.")]
        public string Tenant { get; set; }
        
        public string ClientToken { get; set; }
    }
}