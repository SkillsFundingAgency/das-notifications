using System;
using System.Web.Http;
using NLog.Targets;

namespace SFA.DAS.Notifications.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static RedisTarget _redisTarget; // Required to ensure assembly is copied to output.

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
