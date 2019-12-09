using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.Notifications.Api2.DependencyResolution;
using SFA.DAS.Notifications.Api2.Security;
using Swashbuckle.AspNetCore.Swagger;
using NServiceBus;
using SFA.DAS.Configuration;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;


namespace SFA.DAS.Notifications.Api2
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddADAuthentication(Configuration); replaced with below
            services.AddMvc(options => {
                if (!Environment.IsDevelopment())
                {
                    options.Filters.Add(new AuthorizeFilter("default"));
                }
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0.1", new Info { Title = "Notifications-Api", Version = "v0.1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            //var debugTenant = Configuration.GetSection("auth")["idaTenant"];
            //var debugAudience = Configuration.GetSection("auth")["idaAudience"];

            services.AddDefaultServices();
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            //{
            //    o.TokenValidationParameters = new TokenValidationParameters {
            //        ValidAudience = Configuration.GetSection("auth")["idaAudience"],
            //        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("THIS IS THE SIGNING SECRET")) //todo dev hardcoded
            //    };
            //    o.Authority = Configuration.GetSection("auth")["idaTenant"];
            //    o.Audience = Configuration.GetSection("auth")["idaAudience"];
            //    if (Environment.IsDevelopment()) { o.RequireHttpsMetadata = false; }
            //});

            services.AddNServiceBus(BuildNServiceBusConfiguration());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v0.1/swagger.json", "Notifications Api V0.1");
                c.RoutePrefix = string.Empty;
            });
        }

        private EndpointConfiguration BuildNServiceBusConfiguration()
        {
            var isDevelopment = System.Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName).IsDevelopment();

            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Notifications.MessageHandlers.TestHarness")
                .UseErrorQueue()
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            if (isDevelopment)
            {
                endpointConfiguration.UseLearningTransport(s => s.AddRouting());
            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(Configuration.GetSection("NServiceBusConfiguration")["ServiceBusConnectionString"], s => s.AddRouting());
            }

            return endpointConfiguration;
        }
    }
}
