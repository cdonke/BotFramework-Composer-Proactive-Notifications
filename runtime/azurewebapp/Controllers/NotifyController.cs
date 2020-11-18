using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.BotFramework.Composer.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotFramework.Composer.WebApp.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly IActivityStorage _conversationReferences;

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration, IActivityStorage conversationReferences)
        {
            _adapter = adapter;
            _conversationReferences = conversationReferences;
            _appId = configuration["MicrosoftAppId"];

            // If the channel is the Emulator, and authentication is not in use,
            // the AppId will be null.  We generate a random AppId for this case only.
            // This is not required for production, since the AppId will have a value.
            if (string.IsNullOrEmpty(_appId))
            {
                _appId = Guid.NewGuid().ToString(); //if no AppId, use a random Guid
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_conversationReferences.Keys);
        }

        [HttpPost("{conversationId}")]
        public async Task<IActionResult> Post(string conversationId, [FromBody]Model message)
        {
            var conversationReference = _conversationReferences[conversationId];
            await ((BotAdapter)_adapter).ContinueConversationAsync(
                _appId, 
                conversationReference, 
                (tc, ct) => BotCallback(tc, message.Message, ct), // Or Only the delegate BotCallback
                default(CancellationToken));

            return new ContentResult()
            {
                Content = "<html><body><h1>Proactive messages have been sent.</h1></body></html>",
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        private async Task BotCallback(ITurnContext turnContext, string message, CancellationToken cancellationToken)
        {
            // If you encounter permission-related errors when sending this message, see
            // https://aka.ms/BotTrustServiceUrl
                await turnContext.SendActivityAsync(message);
        }
    }
}
