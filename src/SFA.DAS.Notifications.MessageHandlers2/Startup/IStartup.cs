using System.Threading.Tasks;

namespace SFA.DAS.Notifications.MessageHandlers2.Startup
{
    public interface IStartup
    {
        Task StartAsync();
        Task StopAsync();
    }
}
