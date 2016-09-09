﻿using System;
using System.Threading.Tasks;

namespace SFA.DAS.Notifications.Domain.Repositories
{
    public interface IRepository<TKey, TEntity> where TEntity : class
    {
        Task<TEntity> Get(TKey id);
        Task<TKey> Save(TEntity entity);
        Task Delete(TEntity entity);
    }
}
