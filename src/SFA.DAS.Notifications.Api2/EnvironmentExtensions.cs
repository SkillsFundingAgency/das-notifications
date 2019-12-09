namespace SFA.DAS.Notifications.Api2
{
    public static class EnvironmentExtensions
    {
        public static bool IsDevelopment(this string environment)
        {
            return environment == null || environment == "LOCAL";
        }
    }
}
