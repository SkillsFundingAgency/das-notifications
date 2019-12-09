using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Notifications.Api2.Security
{
    public static class SecurityServicesCollectionExtensions
    {
        //todo why are we still calling this?
        public static void AddADAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            var activeDirectoryConfig = configuration.GetSection("ActiveDirectory").Get<ActiveDirectoryConfiguration>();

            services.AddAuthorization(o =>
            {
                o.AddPolicy("default", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });
            services.AddAuthentication(auth =>
            {
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(auth =>
            {
                auth.Authority = $"https://login.microsoftonline.com/{activeDirectoryConfig.Tenant}";
                auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidAudiences = new List<string>
                    {
                        activeDirectoryConfig.IdentifierUri,
                        activeDirectoryConfig.AppId
                    }
                };
            });

        }

        //todo experimental suspect not needed, trash from some online article
        //public static IApplicationBuilder UseAzureADBearerAuthentication(
        //        this IApplicationBuilder app,
        //        IConfigurationRoot configuration)
        //    {
        //        var tenant = configuration.GetSection("AzureAD:Tenant").Value;
        //        var azureADInstance = configuration.GetSection("AzureAD:AzureADInstance").Value;
        //        var audience = configuration.GetSection("AzureAD:Audience").Value;
        //        var authority = $"{azureADInstance}{tenant}";


        //        var jwtBearerAuthOptions = new JwtBearerOptions {
        //            Audience = audience,
        //            AutomaticAuthenticate = true,
        //            AutomaticChallenge = true,
        //            Authority = authority
        //        };
        //        app.UseJwtBearerAuthentication(jwtBearerAuthOptions);
        //        return app;
        //    }
    }
}
