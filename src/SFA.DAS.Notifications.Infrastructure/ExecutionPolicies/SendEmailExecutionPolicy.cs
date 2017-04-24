using System;
using NLog;
using Polly;
using SFA.DAS.Notifications.Domain.Http;

namespace SFA.DAS.Notifications.Infrastructure.ExecutionPolicies
{
    [PolicyName(Name)]
    public class SendEmailExecutionPolicy : ExecutionPolicy
    {
        public const string Name = "Send Email Policy";

        private readonly ILogger _logger;
        private readonly Policy TooManyRequestsPolicy;
        private readonly Policy ServiceUnavailablePolicy;

        public SendEmailExecutionPolicy(ILogger logger)
        {
            _logger = logger;

            TooManyRequestsPolicy = Policy.Handle<TooManyRequestsException>().WaitAndRetryForeverAsync((i) => new TimeSpan(0, 0, 10), (ex, ts) => OnRetryableFailure(ex));
            ServiceUnavailablePolicy = CreateAsyncRetryPolicy<ServiceUnavailableException>(5, new TimeSpan(0, 0, 10), OnRetryableFailure);
            RootPolicy = Policy.WrapAsync(TooManyRequestsPolicy, ServiceUnavailablePolicy);
        }

        protected override T OnException<T>(Exception ex)
        {
            _logger.Error(ex, $"Exceeded retry limit - {ex.Message}");
            return default(T);
        }

        private void OnRetryableFailure(Exception ex)
        {
            _logger.Info(ex, $"Error sending email - {ex.Message} - Will retry");
        }
    }
}