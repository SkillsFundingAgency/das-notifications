using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using FluentValidation;
using NLog;

namespace SFA.DAS.Notifications.Api
{
    public class ValidationExceptionHandler : ExceptionHandler
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public override void Handle(ExceptionHandlerContext context)
        {
            if (context.Exception is ValidationException)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                var message = ((ValidationException) context.Exception).Message;
                response.Content = new StringContent(message);
                context.Result = new ValidationErrorResult(context.Request, response);

                _logger.Warn(context.Exception, "Validation error");

                return;
            }

            _logger.Error(context.Exception, "Unhandled exception");

            base.Handle(context);
        }
    }
}
