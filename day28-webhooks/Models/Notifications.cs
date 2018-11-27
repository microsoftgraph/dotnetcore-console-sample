using System.Collections.Generic;
using Newtonsoft.Json;

namespace GraphWebhooks
{
    public class Notifications
    {
        [JsonProperty("value")]
        public IEnumerable<Notification> value { get; set; }
    }
}