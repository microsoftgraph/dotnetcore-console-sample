using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace GraphWebhooks.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private const string _subscriptionsResource = "https://graph.microsoft.com/v1.0/subscriptions";
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly HttpClient _graphClient;
        private readonly NotificationUrl _notificationUrl;

        public SubscriptionsController(ISubscriptionRepository subscriptionRepository, HttpClient graphClient, NotificationUrl notificationUrl)
        {
            if (subscriptionRepository == null) throw new ArgumentNullException(nameof(subscriptionRepository));
            if (graphClient == null) throw new ArgumentNullException(nameof(graphClient));
            if (notificationUrl == null) throw new ArgumentNullException(nameof(notificationUrl));
            _subscriptionRepository = subscriptionRepository;
            _graphClient = graphClient;
            _notificationUrl = notificationUrl;
        }

        // GET api/subscriptions/alias@domain.com
        [HttpGet("{upn}")]
        public async Task<ActionResult<Subscription>> Get(string upn)
        {
            var result = _subscriptionRepository.LoadByUpn(upn);
            if (result != null && result.ExpirationDateTime > DateTime.Now)
            {
                return result;
            }
            string clientState = Guid.NewGuid().ToString("d");
            var request = new Subscription
            {
                ChangeType = "created",
                ExpirationDateTime = DateTime.Now.AddDays(2),
                ClientState = clientState,
                Resource = $"users/{upn}/events",
                NotificationUrl = _notificationUrl.Url
            };
            var response = await _graphClient.PostAsJsonAsync(_subscriptionsResource, request);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.ReasonPhrase);
                var error = new ObjectResult(responseBody);
                error.StatusCode = 500;
                return error;
            }
            Subscription subscription = JsonConvert.DeserializeObject<Subscription>(responseBody);
            _subscriptionRepository.Save(subscription);
            return subscription;
        }

        // DELETE api/subscriptions/a7aebd9c-1f8b-41a0-a973-47b7296975c3
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _graphClient.DeleteAsync($"{_subscriptionsResource}/{id}");
            string responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.ReasonPhrase);
                var error = new ObjectResult(responseBody);
                error.StatusCode = 500;
                return error;
            }
            _subscriptionRepository.Delete(id);
            return new StatusCodeResult(204);
        }
    }
}
