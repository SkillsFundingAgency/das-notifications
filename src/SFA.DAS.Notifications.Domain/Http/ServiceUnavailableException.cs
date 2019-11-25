namespace SFA.DAS.Notifications.Domain2.Http
{
    public class ServiceUnavailableException : HttpException
    {
        public ServiceUnavailableException()
            : base(503, "Service is unavailable")
        {
        }
    }
}