using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using NLog;
using Owin;
using SFA.DAS.Configuration;
using SFA.DAS.Notifications.Api;
using SFA.DAS.Notifications.Api.DependencyResolution;
using SFA.DAS.Notifications.Application;
using SFA.DAS.Notifications.Application.Extensions;

[assembly: OwinStartup(typeof(Startup))]
namespace SFA.DAS.Notifications.Api
{
    public class Startup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public void Configuration(IAppBuilder app)
        {
            Logger.Debug("Started running Owin Configuration for API");

            var container = IoC.Initialize();
            var config = new HttpConfiguration
            {
                DependencyResolver = new StructureMapWebApiDependencyResolver(container)
            };

            WebApiConfig.Register(config);

            var configurationService = container.GetInstance<IConfigurationService>();

            configurationService.GetAsync<NotificationServiceConfiguration>().ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        task.Exception.UnpackAndLog(Logger);
                        throw task.Exception.InnerExceptions[0];
                    }
                    var configuration = task.Result;
                    ConfigureAuth(app, configuration.PortalJwtToken);
                }
                ).Wait();


            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}
