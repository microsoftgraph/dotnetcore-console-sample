using day30Sample.helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace day30Sample.Helpers
{

    public static class GraphSdkHelper
    {
        // Mock way to get SDK Clients
        // Set a token here to test this demo.
        public static GraphServiceClient GetAuthenticatedGraphClient(string token)
        {
            var graphClient = new GraphServiceClient(
                 new DelegateAuthenticationProvider(
                   (requestMessage) =>
                   {
                       requestMessage.Headers.Authorization =
                           new AuthenticationHeaderValue("Bearer", token);
                       return Task.FromResult(0);
                   })
               );
            return graphClient;
        }

    }
}
