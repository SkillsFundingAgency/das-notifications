using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.Configuration;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Repositories;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.AzureMessageNotificationRepository
{
    public class AzureEmailNotificationRepository : INotificationsRepository
    {
        private readonly CloudStorageAccount _storageAccount;
        private string _tableName;

        public AzureEmailNotificationRepository(IConfigurationService configurationService)
            : this(configurationService, CloudConfigurationManager.GetSetting("StorageConnectionString")) {}

        public AzureEmailNotificationRepository(IConfigurationService configurationService, string storageConnectionString)
        {
            if (configurationService == null)
                throw new ArgumentNullException(nameof(configurationService));

            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _tableName = configurationService.Get<NotificationServiceConfiguration>().NotificationsStorageConfiguration.TableName;
        }

        public async Task Create(Notification message)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(_tableName);

            var entity = new EmailMessageTableEntity(message.MessageType, message.MessageId)
            {
                Data = JsonConvert.SerializeObject(message.Content)
            };

            var insertOperation = TableOperation.Insert(entity);

            await table.ExecuteAsync(insertOperation);
        }

        public async Task<Notification> Get(string messageType, string messageId)
        {
            var messageData = new Notification
            {
                MessageType = messageType,
                MessageId = messageId,
                Content = null
            };

            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_tableName);

            var tableOperation = TableOperation.Retrieve<EmailMessageTableEntity>(messageType, messageId);
            var result = await table.ExecuteAsync(tableOperation);

            var messageEntity = (EmailMessageTableEntity) result.Result;

            if (messageEntity != null)
                messageData.Content = JsonConvert.DeserializeObject<NotificationContent>(messageEntity.Data);

            return messageData;
        }
    }
}
