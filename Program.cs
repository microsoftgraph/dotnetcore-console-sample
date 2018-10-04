using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleGraphTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientId = "80609421-5b89-47eb-a42c-5aacd3ef8943";
            var clientSecret = "szjwBD9167^=@bwmXZXXV2-";
            var redirectUri = "https://localhost:8042";
            var authority = "https://login.microsoftonline.com/d05889e3-29af-4ce4-8312-9029d4c26b1d/v2.0";
            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var cca = new ConfidentialClientApplication(clientId, authority, redirectUri, new ClientCredential(clientSecret), null, null);
            var authResult = cca.AcquireTokenForClientAsync(scopes).Result;


            //Query using Graph SDK (preferred when possible)
            GraphServiceClient graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("bearer", authResult.AccessToken);

                return Task.FromResult(0);
            }));

            List<QueryOption> options = new List<QueryOption>
            {
                new QueryOption("$top", "1")
            };

            var graphResult = graphServiceClient.Users.Request(options).GetAsync().Result;
            Console.WriteLine(graphResult);


            //Direct query using HTTPClient (for beta endpoint calls or not available in Graph SDK)
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "bearer " + authResult.AccessToken);

            Uri Uri = new Uri("https://graph.microsoft.com/v1.0/users?$top=1");
            var httpResult = client.GetStringAsync(Uri).Result;

            Console.WriteLine(httpResult);
        }
    }
}
