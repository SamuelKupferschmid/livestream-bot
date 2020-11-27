﻿using LivestreamBot.Core.Environment;

using Microsoft.Azure.Cosmos.Table;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LivestreamBot.Persistance
{
    public interface ITableStorage<TEntity>
        where TEntity : TableEntity, new()
    {
        Task InsertOrMergeAsync(TEntity entity);
        IQueryable<TEntity> Get();
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }


    public class TableStorage<TEntity> : ITableStorage<TEntity>
        where TEntity : TableEntity, new()
    {
        private readonly CloudTableClient client;
        private readonly CloudTable table;

        public TableStorage(IAppConfig appConfig)
        {
            var entityName = typeof(TEntity).Name;

            var account = CloudStorageAccount.Parse(appConfig.AzureWebJobsStorage);
            this.client = account.CreateCloudTableClient();

            this.table = client.GetTableReference(entityName);
            this.table.CreateIfNotExists();
        }

        public async Task InsertOrMergeAsync(TEntity entity)
        {
            // TODO pass CancellationToken
            TableOperation operation = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(operation);
        }

        public IQueryable<TEntity> Get()
        {
            return table.CreateQuery<TEntity>();
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var operation = TableOperation.Delete(entity);
            await table.ExecuteAsync(operation, cancellationToken);
        }
    }
}
