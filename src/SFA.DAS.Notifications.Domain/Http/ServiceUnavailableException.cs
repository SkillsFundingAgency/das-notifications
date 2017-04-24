namespace SFA.DAS.Notifications.Domain.Http
{
    public class ServiceUnavailableException : HttpException
    {
        public ServiceUnavailableException()
            : base(500, "Service is unavailable")
        {
        }
    }
}