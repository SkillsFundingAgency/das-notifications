namespace SFA.DAS.Notifications.Api.Security
{
    public class ApiAuthentication
    {
        public string ApiTokenSecret { get; set; }

        public string ApiIssuer { get; set; }

        public string ApiAudiences { get; set; }
    }
}
