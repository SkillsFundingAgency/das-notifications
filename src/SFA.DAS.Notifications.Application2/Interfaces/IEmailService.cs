using System.Threading.Tasks;
using SFA.DAS.Notifications.Domain2.Entities;

namespace SFA.DAS.Notifications.Application2.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message);
    }
}
