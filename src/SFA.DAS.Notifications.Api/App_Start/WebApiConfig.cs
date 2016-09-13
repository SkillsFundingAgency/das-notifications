using System;
using System.Web.Http;

namespace SFA.DAS.Notifications.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
        }
    }
}
