using System;
using Microsoft.Extensions.Logging;
using Polly;
using SFA.DAS.Notifications.Domain.Http;

namespace SFA.DAS.Notifications.Infrastructure.ExecutionPolicies
{
    [PolicyName(Name)]
    public class SendMessageExecutionPolicy : ExecutionPolicy
    {
        public const string Name = "Send message policy";

        private readonly ILogger<SendMessageExecutionPolicy> _logger;

        public SendMessageExecutionPolicy(ILogger<SendMessageExecutionPolicy> logger)
        {
            _logger = logger;

            var TooManyRequestsPolicy = Policy.Handle<TooManyRequestsException>().WaitAndRetryForeverAsync((i) => new TimeSpan(0, 0, 10), (ex, ts) => OnRetryableFailure(ex));
            var ServiceUnavailablePolicy = CreateAsyncRetryPolicy<ServiceUnavailableException>(5, new TimeSpan(0, 0, 10), OnRetryableFailure);
            var InternalServerErrorPolicy = CreateAsyncRetryPolicy<InternalServerErrorException>(5, new TimeSpan(0, 0, 10), OnRetryableFailure);
            RootPolicy = Policy.WrapAsync(TooManyRequestsPolicy, ServiceUnavailablePolicy, InternalServerErrorPolicy);
        }

        protected override T OnException<T>(Exception ex)
        {
            _logger.LogError(ex, $"Exceeded retry limit - {ex.Message}");
            return default(T);
        }

        private void OnRetryableFailure(Exception ex)
        {
            _logger.LogInformation(ex, $"Error sending email - {ex.Message} - Will retry");
        }
    }
}