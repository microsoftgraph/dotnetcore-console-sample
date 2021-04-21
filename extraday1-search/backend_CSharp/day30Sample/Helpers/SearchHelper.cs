using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using day30Sample.Helpers;
using Microsoft.Graph;

namespace day30Sample.helpers
{
    public class SearchHelper
    {
        public static async Task<SearchResponse> Search(List<SearchRequestObject> sro, string token)
        {
            GraphServiceClient graphClient = GraphSdkHelper.GetAuthenticatedGraphClient(token);
            return await graphClient.Search.Query(sro).Request().PostAsync();
        }
    }
}