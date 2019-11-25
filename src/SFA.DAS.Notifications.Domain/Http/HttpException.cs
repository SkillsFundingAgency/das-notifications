using System;

namespace SFA.DAS.Notifications.Domain.Http
{
    public class HttpException : Exception
    {
        public HttpException(int statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
        public HttpException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}
