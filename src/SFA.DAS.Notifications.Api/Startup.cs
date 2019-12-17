using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.Notifications.Api.DependencyResolution;
using SFA.DAS.Notifications.Api.Security;
using Swashbuckle.AspNetCore.Swagger;
using NServiceBus;
using SFA.DAS.Configuration;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using System.Collections;
using System.Collections.Generic;

namespace SFA.DAS.Notifications.Api
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
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme {
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

            services.AddDefaultServices();

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
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenManagement.Secret)),
                    ValidIssuer = tokenManagement.Issuer,
                    ValidAudience = tokenManagement.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            var ourTestJwt = GenToken(tokenManagement);

            services.AddNServiceBus(BuildNServiceBusConfiguration());
        }

        private string GenToken(TokenManagement tokenManagement)
        {
            var token = string.Empty;

            var claim = new[]
            {
                new Claim(ClaimTypes.Name, "test user")
            };
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                tokenManagement.Issuer,
                tokenManagement.Audience,
                claim,
                expires: DateTime.Now.AddMinutes(tokenManagement.AccessExpiration),
                signingCredentials: credentials
            );
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
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
