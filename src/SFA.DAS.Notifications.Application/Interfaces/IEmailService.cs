using System;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message);
    }
}
