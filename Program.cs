using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;

namespace ConsoleGraphTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load appsettings.json
            var config = LoadAppSettings();
            if (null == config)
            {
                Console.WriteLine("Missing or invalid appsettings.json file. Please see README.md for configuration instructions.");
                return;
            }

            var clientId = config["applicationId"];
            var clientSecret = config["applicationSecret"];
            var redirectUri = "https://localhost:8042";
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}/v2.0";
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

        private static IConfigurationRoot LoadAppSettings()
        {
            try
            {
                var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

                // Validate required settings
                if (string.IsNullOrEmpty(config["applicationId"]) ||
                    string.IsNullOrEmpty(config["applicationSecret"]) ||
                    string.IsNullOrEmpty(config["tenantId"]))
                {
                    return null;
                }

                return config;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
        }
    }
}
