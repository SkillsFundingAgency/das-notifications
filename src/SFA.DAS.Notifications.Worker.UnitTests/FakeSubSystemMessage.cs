using System;
using Newtonsoft.Json;
using SFA.DAS.Messaging;
using SFA.DAS.Notifications.Application.Messages;

namespace SFA.DAS.Notifications.Worker.UnitTests
{
    public class FakeSubSystemMessage : SubSystemMessage
    {
        public FakeSubSystemMessage(QueueMessage content)
        {
            base.Content = JsonConvert.SerializeObject(content);
        }
    }
}