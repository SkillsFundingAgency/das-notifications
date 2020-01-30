namespace SFA.DAS.Notifications.Api
{
    public static class EnvironmentExtensions
    {
        public static bool IsDevelopment(this string environment)
        {
            return environment == null || environment == "LOCAL";
        }
    }
}
