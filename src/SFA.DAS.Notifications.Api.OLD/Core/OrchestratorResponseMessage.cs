using System;

namespace SFA.DAS.Notifications.Api.Core
{
    public class OrchestratorResponseMessage
    {
        public string Text { get; set; }

        public UserMessageLevel Level { get; set; }
    }
}