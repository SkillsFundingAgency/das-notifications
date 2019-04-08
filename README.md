# Notifications API #

This API provides a mechanism for send email or SMS messages to users of the Digital Apprenticeship Service.

The API wraps underlying email and SMS service providers such as SMTP services, SendGrid, Notify, etc.

**Build status**

![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/148/badge)


## Functionality ##

### Sending emails ###

To send a new email:

POST http://host:port/api/email/

    {
        "templateId": "email_template_id",
        "subject": "A test email",
        "recipientsAddress": "user@email.com",
        "replyToAddress": "noreply@service.com",
        "tokens": {
            "Key1": "value1",
            "Key2": "value2",
            "Key3": "value3"
        }
    }

Where:

- **templateId** is the identifer for the email template to be used (eg. a SendGrid template ID)
- **subject** is the email subject
- **recipientsAddress** is the email address to send the message to
- **replyToAddress** is the "reply to" address that will show as the sender
- **tokens** is an array of key/value pairs which are used to replace placeholders in the email body


## Security ##

The API uses Azure Active Directory to enforce authorised access to the API methods. When not running locally, callers must have a valid client id and secret.

Supported permissions are:

    SendEmail
    SendSMS


## Additional Gov Notify Services ##

The API now supports multiple Gov Notify services. To use a different service, pass a SystemId with the email or SMS that matches the ServiceName in the configuration described below. If the system id is blank or not found in configuration then the default Apprenticeship Service api key will be used.

To configure your service, add an element to the ConsumerConfiguration array inside `"NotifyServiceConfiguration"` in DAS Employer Config. This can be done via the build pipeline using the environment variable`GovNotifyConsumerConfiguration`

```
"ConsumerConfiguration": [
    {
      "ServiceName": "YourSystemId",
      "ApiKey": "samplekey-11111111-1111-1111-1111-111111111111-22222222-2222-2222-2222-222222222222"
    }
  ]
```

The API Key should be a V2 API Key obtained from the Gov Notify portal.
