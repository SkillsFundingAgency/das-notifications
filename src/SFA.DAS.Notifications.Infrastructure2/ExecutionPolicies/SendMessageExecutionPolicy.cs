using System;
using NLog;
using Polly;
using SFA.DAS.Notifications.Domain2.Http;

namespace SFA.DAS.Notifications.Infrastructure2.ExecutionPolicies
{
    [PolicyName(Name)]
    public class SendMessageExecutionPolicy : ExecutionPolicy
    {
        public const string Name = "Send message policy";

        private readonly ILogger _logger;
        private readonly Policy TooManyRequestsPolicy;
        private readonly Policy ServiceUnavailablePolicy;
        private readonly Policy InternalServerErrorPolicy;

        public SendMessageExecutionPolicy(ILogger logger)
        {
            _logger = logger;

            TooManyRequestsPolicy = Policy.Handle<TooManyRequestsException>().WaitAndRetryForeverAsync((i) => new TimeSpan(0, 0, 10), (ex, ts) => OnRetryableFailure(ex));
            ServiceUnavailablePolicy = CreateAsyncRetryPolicy<ServiceUnavailableException>(5, new TimeSpan(0, 0, 10), OnRetryableFailure);
            InternalServerErrorPolicy = CreateAsyncRetryPolicy<InternalServerErrorException>(5, new TimeSpan(0, 0, 10), OnRetryableFailure);
            RootPolicy = Policy.WrapAsync(TooManyRequestsPolicy, ServiceUnavailablePolicy, InternalServerErrorPolicy);
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