using Newtonsoft.Json;

namespace GraphWebhooks
{
    public class ResourceData
    {
        [JsonProperty("@odata.type")]
        public string OdataType { get; set; }

        [JsonProperty("@odata.id")]
        public string OdataId { get; set; }

        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}