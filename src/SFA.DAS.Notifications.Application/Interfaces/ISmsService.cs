using System;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Application.Messages;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Application.Interfaces
{
    public interface ISmsService
    {
        Task SendAsync(SmsMessage message); //todo: should use domain entities
    }
}
