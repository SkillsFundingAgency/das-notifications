using System;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Application.Messages;

namespace SFA.DAS.Notifications.Application.Interfaces
{
    public interface ISmsService
    {
        Task SendAsync(SmsMessage message); //todo: should use domain entities
    }
}
