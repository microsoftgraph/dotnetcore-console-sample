using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;

namespace ConsoleGraphTest
{
    class Program
    {
        private static GraphServiceClient _graphServiceClient;
        private static HttpClient _httpClient;

        static void Main(string[] args)
        {
            // Load appsettings.json
            var config = LoadAppSettings();
            if (null == config)
            {
                Console.WriteLine("Missing or invalid appsettings.json file. Please see README.md for configuration instructions.");
                return;
            }

            //Query using Graph SDK (preferred when possible)
            GraphServiceClient graphClient = GetAuthenticatedGraphClient(config);
            List<QueryOption> options = new List<QueryOption>
            {
                new QueryOption("$top", "1")
            };

            var graphResult = graphClient.Users.Request(options).GetAsync().Result;
            Console.WriteLine("Graph SDK Result");
            Console.WriteLine(graphResult[0].DisplayName);

            //Direct query using HTTPClient (for beta endpoint calls or not available in Graph SDK)
            HttpClient httpClient = GetAuthenticatedHTTPClient(config);
            Uri Uri = new Uri("https://graph.microsoft.com/v1.0/users?$top=1");
            var httpResult = httpClient.GetStringAsync(Uri).Result;

            Console.WriteLine("HTTP Result");
            Console.WriteLine(httpResult);

            //Below methods showcase MS Graph sdk usage with mailbox
            ListUserMailInboxMessages();
            GetUserMailboxDefaultTimeZone();
            CreateUserMailBoxRule();
            ListUserMailBoxRules();
        }

        // Add a private method to do any necessary setup and make calls to your helper
        private static void ListUserMailInboxMessages()
        {
            const string alias = "admin";
            var mailboxHelper = new MailboxHelper(_graphServiceClient);
            List<ResultsItem> items = mailboxHelper.ListInboxMessages(alias).Result;
            Console.WriteLine("Message count: "+ items.Count);
        }
   
        private static void GetUserMailboxDefaultTimeZone()
        {
            const string alias = "admin";
            var mailboxHelper = new MailboxHelper(_graphServiceClient);
            var defaultTimeZone = mailboxHelper.GetUserMailboxDefaultTimeZone(alias).Result;
            Console.WriteLine("Default timezone: "+ defaultTimeZone);
        }
        private static void ListUserMailBoxRules()
        {
            const string alias = "admin";
            var mailboxHelper = new MailboxHelper(_graphServiceClient);
            List<ResultsItem> rules = mailboxHelper.GetUserMailboxRules(alias).Result;
            Console.WriteLine("Rules count: "+ rules.Count);
            foreach(ResultsItem rule in rules)
            {
                Console.WriteLine("Rule Name: "+ rule.Display);
            }
        }

        private static void CreateUserMailBoxRule()
        {
            const string alias = "admin";
            var mailboxHelper = new MailboxHelper(_graphServiceClient);
            mailboxHelper.CreateRule(alias, "ForwardBasedonSender", 2, true, "svarukal", "adelev@M365x995052.onmicrosoft.com").GetAwaiter().GetResult();
        }
        private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            _graphServiceClient = new GraphServiceClient(authenticationProvider);
            return _graphServiceClient;
        }

        private static HttpClient GetAuthenticatedHTTPClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            _httpClient = new HttpClient(new AuthHandler(authenticationProvider, new HttpClientHandler()));
            return _httpClient;
        }

        private static IAuthenticationProvider CreateAuthorizationProvider(IConfigurationRoot config)
        {
            var clientId = config["applicationId"];
            var clientSecret = config["applicationSecret"];
            var redirectUri = config["redirectUri"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}/v2.0";

            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var cca = new ConfidentialClientApplication(clientId, authority, redirectUri, new ClientCredential(clientSecret), null, null);
            return new MsalAuthenticationProvider(cca, scopes.ToArray());
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
                    string.IsNullOrEmpty(config["redirectUri"]) ||
                    string.IsNullOrEmpty(config["tenantId"]) ||
                    string.IsNullOrEmpty(config["domain"]))
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
