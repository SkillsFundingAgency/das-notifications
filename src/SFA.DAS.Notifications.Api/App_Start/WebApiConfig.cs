using System.Configuration;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using SFA.DAS.ApiTokens.Client;

namespace SFA.DAS.Notifications.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var apiKeySecret = ConfigurationManager.AppSettings["ApiTokenSecret"];
            var apiIssuer = ConfigurationManager.AppSettings["ApiIssuer"];
            var apiAudiences = ConfigurationManager.AppSettings["ApiAudiences"].Split(' ');

            config.MessageHandlers.Add(new ApiKeyHandler("Authorization", apiKeySecret, apiIssuer, apiAudiences));

            config.MapHttpAttributeRoutes();

            config.Services.Replace(typeof(IExceptionHandler), new ValidationExceptionHandler());
        }
    }
}
