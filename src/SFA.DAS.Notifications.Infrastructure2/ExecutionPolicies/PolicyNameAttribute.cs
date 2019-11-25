using System;

namespace SFA.DAS.Notifications.Infrastructure2.ExecutionPolicies
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PolicyNameAttribute : Attribute
    {
        public PolicyNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}