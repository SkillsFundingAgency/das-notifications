using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.Configuration;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Repositories;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.AzureMessageNotificationRepository
{
    public class AzureEmailNotificationRepository : INotificationsRepository
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly string _tableName;

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
            var notification = new Notification
            {
                MessageType = messageType,
                MessageId = messageId,
                Content = null
            };

            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_tableName);

            var tableOperation = TableOperation.Retrieve<EmailMessageTableEntity>(messageType, messageId);
            var result = await table.ExecuteAsync(tableOperation);

            var tableEntity = (EmailMessageTableEntity) result.Result;

            if (tableEntity != null)
                notification.Content = JsonConvert.DeserializeObject<NotificationContent>(tableEntity.Data);

            return notification;
        }
    }
}
