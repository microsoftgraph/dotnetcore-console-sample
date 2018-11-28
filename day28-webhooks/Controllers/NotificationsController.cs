using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace GraphWebhooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly HttpClient _graphClient;

        public NotificationsController(ISubscriptionRepository subscriptionRepository, HttpClient graphClient)
        {
            if (subscriptionRepository == null) throw new ArgumentNullException(nameof(subscriptionRepository));
            if (graphClient == null) throw new ArgumentNullException(nameof(graphClient));
            _subscriptionRepository = subscriptionRepository;
            _graphClient = graphClient;
        }

        [HttpPost]
        public async Task<ActionResult> Listen([FromQuery] string validationToken)
        {
            if (!string.IsNullOrEmpty(validationToken))
            {
                return Content(validationToken, "plain/text");
            }
            try
            {
                // Read the post body directly as we can't mix optional FromBody and FromQuery parameters
                var postBody = await Request.GetBodyAsync<Notifications>();
                foreach (var item in postBody.value)
                {
                    await ProcessEventNotification(item);
                }
            }
            catch (Exception)
            {
                // Just ignore exceptions
            }
            // Send a 202 so MicrosoftGraph knows we processed the notification
            return new StatusCodeResult(202);
        }

        private async Task ProcessEventNotification(Notification item)
        {
            var subscription = _subscriptionRepository.Load(item.SubscriptionId);
            // We should only process requests for which we have ClientState stored
            if (subscription != null && item.ClientState == subscription.ClientState)
            {
                Uri Uri = new Uri($"https://graph.microsoft.com/v1.0/{item.Resource}");
                var httpResult = await _graphClient.GetStringAsync(Uri);
                var calendarEvent = JsonConvert.DeserializeObject<Event>(httpResult);
                // Do processing of your subscribed entity
                Console.WriteLine(httpResult);
                if (string.IsNullOrWhiteSpace(calendarEvent.BodyPreview))
                {
                    // Decline the meeting as it has no agenda
                }
            }
        }
    }
}