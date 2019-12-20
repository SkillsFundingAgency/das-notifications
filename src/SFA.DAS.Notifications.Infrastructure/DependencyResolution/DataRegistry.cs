using Microsoft.Extensions.Configuration;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace SFA.DAS.Notifications.Infrastructure.DependencyResolution
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            For<DbConnection>().Use(c => new SqlConnection(c.GetInstance<IConfiguration>().GetNotificationSection<NotificationServiceConfiguration>().DatabaseConnectionString));
        }
    }

}