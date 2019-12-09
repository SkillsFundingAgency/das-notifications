using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Notifications.Api2.Core
{
    public class OrchestratorResponse
    {
        public string Code { get; set; }

        public ValidationResult ValidationResult { get; set; }
    }
}
