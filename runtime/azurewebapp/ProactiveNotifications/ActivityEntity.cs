using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.BotFramework.Composer.WebApp.ProactiveNotifications
{
    public class ActivityEntity : TableEntity
    {
        public ActivityEntity()
        {
            this.PartitionKey = "Activities";
        }

        public ActivityEntity(string id, ConversationReference conversationReference) 
            : this()
        {
            RowKey = id;
            ConversationReference = JsonConvert.SerializeObject(conversationReference);
        }

        public string ConversationReference { get; set; }
    }
}
