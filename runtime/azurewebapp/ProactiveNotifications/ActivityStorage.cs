using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Bot.Schema;
using Microsoft.BotFramework.Composer.Core.Interfaces;
using Microsoft.BotFramework.Composer.Core.Settings;
using Newtonsoft.Json;

namespace Microsoft.BotFramework.Composer.WebApp.ProactiveNotifications
{
    public class ActivityStorage : IActivityStorage
    {
        private readonly CloudTable _table;

        public ActivityStorage(BotSettings botSettings)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(botSettings.BlobStorage.ConnectionString);
            var tableClient = cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference("UserActivity");

            table.CreateIfNotExists();
            _table = table;
        }

        public string[] Keys
        {
            get
            {
                var query = new TableQuery<ActivityEntity>();
                var conversations = _table.ExecuteQuery(query);
                return conversations.Select(q => q.RowKey).ToArray();
            }
        }

        public ConversationReference this[string id] => Get(id);

        public async Task AddOrUpdate(string id, ConversationReference conversationReference)
        {
            var item = new ActivityEntity(id, conversationReference);
            var operation = TableOperation.InsertOrReplace(item);

            await _table.ExecuteAsync(operation);
        }

        public ConversationReference Get(string id)
        {
            var query = new TableQuery<ActivityEntity>();
            query.FilterString = $"RowKey eq '{id}'";
            query.TakeCount = 1;

            var conversations = _table.ExecuteQuery(query);

            return JsonConvert.DeserializeObject<ConversationReference>(conversations.FirstOrDefault().ConversationReference);
        }
    }
}
