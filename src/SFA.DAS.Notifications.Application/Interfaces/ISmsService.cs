using System.Threading.Tasks;
using SFA.DAS.Notifications.Domain2.Entities;

namespace SFA.DAS.Notifications.Application.Interfaces
{
    public interface ISmsService
    {
        Task SendAsync(SmsMessage message);
    }
}