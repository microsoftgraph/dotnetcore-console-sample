using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace day30Sample.Helpers
{
    public class EventHelper
    {
        public static async Task<Event> GetEventByEventId(string id, string token)
        {

            GraphServiceClient graphClient = GraphSdkHelper.GetAuthenticatedGraphClient(token);
            return await graphClient.Me.Events[id].Request().GetAsync();

        }
        
    }
}
