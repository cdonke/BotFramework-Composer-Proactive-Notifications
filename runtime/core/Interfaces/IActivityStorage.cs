using Microsoft.Bot.Schema;
using System.Threading.Tasks;

namespace Microsoft.BotFramework.Composer.Core.Interfaces
{
    public interface IActivityStorage
    {
        Task AddOrUpdate(string id, ConversationReference conversationReference);
        ConversationReference Get(string id);
        string[] Keys { get; }
        ConversationReference this[string id] { get; }
    }
}
