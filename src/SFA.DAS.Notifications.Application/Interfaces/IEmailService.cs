using System;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Application.Messages;

namespace SFA.DAS.Notifications.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message); //todo: should use domain entities
    }
}
