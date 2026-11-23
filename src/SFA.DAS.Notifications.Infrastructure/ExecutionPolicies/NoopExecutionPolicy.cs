using System;
using System.Threading.Tasks;

namespace SFA.DAS.Notifications.Infrastructure.ExecutionPolicies
{
    public class NoopExecutionPolicy : ExecutionPolicy
    {
        
        public override async Task ExecuteAsync(Func<Task> action)
        {
            await action();
        }

        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            return await func();
        }
    }
}