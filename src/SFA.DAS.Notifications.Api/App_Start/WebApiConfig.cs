using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Microsoft.Azure;
using SFA.DAS.ApiTokens.Client;

namespace SFA.DAS.Notifications.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Services.Replace(typeof(IExceptionHandler), new ValidationExceptionHandler());
        }
    }
}
