using System;

namespace SFA.DAS.Notifications.Api.Core
{
    public abstract class OrchestratorBase
    {
        protected static OrchestratorResponse GetOrchestratorResponse(string code)
        {
            return new OrchestratorResponse
            {
                Code = code
            };
        }
    }
}
