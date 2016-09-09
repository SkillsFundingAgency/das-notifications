using System;

namespace SFA.DAS.Notifications.Application.Services
{
    public class DefaultGuidProvider : GuidProvider
    {
        public override Guid NewGuid()
        {
            return Guid.NewGuid();
        }
    }
}