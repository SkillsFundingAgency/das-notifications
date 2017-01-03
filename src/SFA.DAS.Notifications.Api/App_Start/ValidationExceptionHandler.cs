using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using FluentValidation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Notifications.Api
{
    public class ValidationExceptionHandler : ExceptionHandler
    {
        private static readonly ILog Logger = new NLogLogger();

        public override void Handle(ExceptionHandlerContext context)
        {
            if (context.Exception is ValidationException)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                var message = ((ValidationException) context.Exception).Message;
                response.Content = new StringContent(message);
                context.Result = new ValidationErrorResult(context.Request, response);

                Logger.Warn(context.Exception, "Validation error");

                return;
            }

            Logger.Error(context.Exception, "Unhandled exception");

            base.Handle(context);
        }
    }
}
