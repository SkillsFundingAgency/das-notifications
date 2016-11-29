using System.Threading.Tasks;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Domain.Repositories
{
    public interface INotificationsRepository
    {
        Task Create(Notification message);
        Task<Notification> Get(NotificationFormat format, string messageId);
        Task Update(NotificationFormat format, string messageId, NotificationStatus sending);
    }
}
