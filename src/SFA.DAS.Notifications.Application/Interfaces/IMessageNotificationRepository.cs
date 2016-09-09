using System;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Application.DataEntities;

namespace SFA.DAS.Notifications.Application.Interfaces
{
    public interface IMessageNotificationRepository
    {
        Task Create(MessageData message);
        Task<MessageData> Get(string messageType, string messageId);
    }
}