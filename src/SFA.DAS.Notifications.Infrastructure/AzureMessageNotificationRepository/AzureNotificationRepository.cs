using System;
using System.Collections.Generic;
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
    public class AzureNotificationRepository : INotificationsRepository
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly string _tableName;

        public AzureNotificationRepository(IConfigurationService configurationService)
            : this(configurationService, CloudConfigurationManager.GetSetting("StorageConnectionString")) {}

        public AzureNotificationRepository(IConfigurationService configurationService, string storageConnectionString)
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

            var entity = new NotificationTableEntity(message.Format, message.MessageId)
            {
                Data = JsonConvert.SerializeObject(message)
            };

            var insertOperation = TableOperation.Insert(entity);

            await table.ExecuteAsync(insertOperation);
        }

        public async Task<Notification> Get(NotificationFormat format, string messageId)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_tableName);

            var tableOperation = TableOperation.Retrieve<NotificationTableEntity>(format.ToString(), messageId, (List<string>)null);
            var result = await table.ExecuteAsync(tableOperation);

            var tableEntity = (NotificationTableEntity) result.Result;

            var notification = JsonConvert.DeserializeObject<Notification>(tableEntity.Data);

            return notification;
        }

        public async Task Update(NotificationFormat format, string messageId, NotificationStatus status)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_tableName);

            var getOperationt= TableOperation.Retrieve<NotificationTableEntity>(format.ToString(), messageId, (List<string>)null);
            var result = await table.ExecuteAsync(getOperationt);

            var notificationTableEntity = (NotificationTableEntity)result.Result;

            var notification = JsonConvert.DeserializeObject<Notification>(notificationTableEntity.Data);
            notification.Status = status;

            notificationTableEntity.Data = JsonConvert.SerializeObject(notification);

            var updateOperation = TableOperation.Replace(notificationTableEntity);

            await table.ExecuteAsync(updateOperation);
        }
    }
}
