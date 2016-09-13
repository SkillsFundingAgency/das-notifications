using System;

namespace SFA.DAS.Notifications.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LoggingConfig.ConfigureLogging();
        }
    }
}
