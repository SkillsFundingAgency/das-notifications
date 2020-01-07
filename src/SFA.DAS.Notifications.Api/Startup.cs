using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Notifications.Api.Orchestrators;
using SFA.DAS.Notifications.Api.Security;
using SFA.DAS.Notifications.Api.StartupExtensions;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SFA.DAS.Notifications.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment hostingContext)
        {
            Configuration = configuration;
            Environment = hostingContext;
            var environmentName = hostingContext.EnvironmentName;

            var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: false)
                    .AddJsonFile($"appSettings.{environmentName}.json", optional: true, reloadOnChange: false)
                    .AddEnvironmentVariables()
                    .AddUserSecrets<Startup>()
                    .AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = new[] { "SFA.DAS.Notifications" };
                        options.EnvironmentNameEnvironmentVariableName = "APPSETTING_EnvironmentName";
                        options.StorageConnectionStringEnvironmentVariableName = "APPSETTING_ConfigurationStorageConnectionString";
                        options.PreFixConfigurationKeys = false;
                    }).Build();

            Configuration = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                if (!Environment.IsDevelopment())
                {
                    options.Filters.Add(new AuthorizeFilter("default"));
                }
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHealthChecks();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0.1", new Info { Title = "Notifications-Api", Version = "v0.1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Please Enter \"Bearer {token}\" into the value box.",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    {"Bearer",new string[]{ } }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddTransient<INotificationOrchestrator, NotificationOrchestrator>();
            services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));
            var tokenManagement = Configuration.GetSection("tokenManagement").Get<TokenManagement>();
            var secret = Encoding.ASCII.GetBytes(tokenManagement.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenManagement.Secret)),
                    ValidIssuer = tokenManagement.Issuer,
                    ValidAudience = tokenManagement.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
        {
            serviceProvider.StartNServiceBus(Configuration, Environment.IsDevelopment());
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
            app.UseAuthentication();
            app.UseMvc();
            app.UseHealthChecks("/api/health");
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v0.1/swagger.json", "Notifications Api V0.1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
