# Notifications API #

This API provides a mechanism for sending email or SMS messages to users of the Digital Apprenticeship Service.

The API sends messages using GOV.UK Notify.

**Build status**

![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/148/badge)

> **Developers:** to set up, run and test this service locally, see the [Developer Guide](docs/DEVELOPER-GUIDE.md).


## Projects ##

- **SFA.DAS.Notifications.Api** - HTTP API for sending emails and SMS messages
- **SFA.DAS.Notifications.MessageHandlers** - NServiceBus endpoint (continuous WebJob) that handles `SendEmailCommand` and `SendSmsCommand` messages and sends them to GOV.UK Notify
- **SFA.DAS.Notifications.MessageHandlers.TestHarness** - console app for sending test messages locally
- **SFA.DAS.Notifications.Database** - SQL database project
- **SFA.DAS.Notifications.Messages** - NuGet package containing `SendEmailCommand` and `SendSmsCommand`


## Functionality ##

### Sending emails ###

To send a new email:

POST http://host:port/api/email/

    {
        "templateId": "email_template_id",
        "recipientsAddress": "user@email.com",
        "tokens": {
            "Key1": "value1",
            "Key2": "value2",
            "Key3": "value3"
        }
    }

Where:

- **templateId** is a GOV.UK Notify template ID, or the `Id` of a template in the `SFA.DAS.Notifications-Templates` configuration
- **recipientsAddress** is the email address to send the message to
- **tokens** is an array of key/value pairs which are used to replace placeholders in the email body

The request also accepts `subject` and `replyToAddress`, but these are not used. The subject and reply-to address are set on the GOV.UK Notify template.

### Sending SMS ###

To send a new SMS:

POST http://host:port/api/sms/

    {
        "templateId": "sms_template_id",
        "recipientsNumber": "07700900000",
        "tokens": {
            "Key1": "value1"
        }
    }

Where:

- **templateId** is the `Id` of a template in the `SFA.DAS.Notifications-Templates` configuration
- **recipientsNumber** is the phone number to send the message to
- **tokens** is an array of key/value pairs which are used to replace placeholders in the message


## Security ##

The API uses JWT bearer tokens to enforce authorised access to the API methods. Tokens are signed with a shared secret and validated against the `ApiAuthentication` configuration (`ApiIssuer`, `ApiAudiences` and `ApiTokenSecret`).

Supported permissions (`role` claim) are:

    SendEmail
    SendSMS


## GOV.UK Notify Services ##

All messages are sent through a single GOV.UK Notify service, using the `NotificationServiceApiKey` in configuration.

The request types still have a `SystemId` property, but it is not used.


# Sending email using NServiceBus #
To set up a dotnet core app to Send Emails.

## App setup ##

Create a config object for NServiceBus

    "NServiceBusConfiguration": {
        "SharedServiceBusEndpointUrl": "Endpoint=sb://das-at-shared-ns.servicebus.windows.net/",
        "NServiceBusLicense": ""
    }

Obtain a license from DevOps and populate **NServiceBusLicense**.

Add references to:

    SFA.DAS.Notifications.Messages
    SFA.DAS.NServiceBus

Add something like this to your app:

    public static class ServiceCollectionExtensions
    {
        private const string EndpointName = "SFA.DAS.[YOUR APP]";

        public static IServiceCollection AddNServiceBus(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var nServiceBusConfiguration = configuration.GetSection("NServiceBusConfiguration").Get<NServiceBusConfiguration>();

            var endpointConfiguration = new EndpointConfiguration(EndpointName)
                .UseErrorQueue($"{EndpointName}-errors")
                .UseLicense(nServiceBusConfiguration.NServiceBusLicense)
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            endpointConfiguration.SendOnly();

            if (environment.IsDevelopment())
            {
                endpointConfiguration.UseLearningTransport(s => s.AddRouting());
            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(nServiceBusConfiguration.SharedServiceBusEndpointUrl, s => s.AddRouting());
            }

            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            return services
                .AddSingleton(endpoint)
                .AddSingleton<IMessageSession>(endpoint)
                .AddHostedService<NServiceBusHostedService>();
        }
    }

    public static class RoutingSettingsExtensions
    {
        private const string NotificationsMessageHandler = "SFA.DAS.Notifications.MessageHandlers";

        public static void AddRouting(this RoutingSettings routingSettings)
        {
            routingSettings.RouteToEndpoint(typeof(SendEmailCommand), NotificationsMessageHandler);
            routingSettings.RouteToEndpoint(typeof(SendSmsCommand), NotificationsMessageHandler);
        }
    }

Then add `services.AddNServiceBus(configuration, environment);` when registering services.


To send an email
Inject `IMessageSession` into your class.

Call:

    await _messageSession.Send(new SendEmailCommand(
                        "Your GOV Notify email template ID", 
                        "Recipient email address", 
                        new Dictionary<string, string>(){{"TokenKey", "TokenValue"}}));

This will allow local development to send messages which will be dropped into a .learningTransport folder in your application's root folder.

## Sending emails from released App ##

For the released app to send emails, devops will have to add a role assignment for the app service that's accessing the NServiceBus like https://github.com/SkillsFundingAgency/das-reservations-api/blob/master/azure/template.json#L246-L273 but with `Sender` instead of `Owner`.

## Sending emails from local dev ##

Devops will need to add the Azure Service Bus Data Sender role to your personal CDS account.

Once that's done, install the Azure CLI tools.

`az login`

(Log in with your CDS creds)

`az account set --subscription [Your subscription ID]`

Either configure your local app to not be development mode, or comment out the code in AddNServiceBus that switches to UseLearningTransport.
