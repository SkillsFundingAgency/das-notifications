﻿using System;
using System.Web.Http;
using NLog.Targets;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Notifications.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly ILog Logger = new NLogLogger();

        private static RedisTarget _redisTarget; // Required to ensure assembly is copied to output.

        protected void Application_Start()
        {
            Logger.Info("Starting Notifications Api Application");
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_End()
        {
            Logger.Info("Stopping Notifications Api Application");
        }

        protected void Application_Error()
        {
            Exception ex = Server.GetLastError().GetBaseException();

            Logger.Error(ex, "Unhandled exception");
        }
    }
}
