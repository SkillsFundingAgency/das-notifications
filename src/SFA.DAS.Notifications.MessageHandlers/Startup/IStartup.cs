using System.Threading.Tasks;

namespace SFA.DAS.Notifications.MessageHandlers.Startup
{
    public interface IStartup
    {
        Task StartAsync();
        Task StopAsync();
    }
}
