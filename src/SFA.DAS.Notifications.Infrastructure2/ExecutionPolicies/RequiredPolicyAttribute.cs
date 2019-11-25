using System;

namespace SFA.DAS.Notifications.Infrastructure2.ExecutionPolicies
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RequiredPolicyAttribute : Attribute
    {
        public RequiredPolicyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
