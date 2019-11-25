namespace SFA.DAS.Notifications.Domain.Http
{
    public class TooManyRequestsException : HttpException
    {
        public TooManyRequestsException()
            : base(429, "Rate limit has been reached")
        {
        }
    }
}