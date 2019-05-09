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
            HttpClient httpClient = GetAuthenticatedHTTPClient(config);
            
            //Below methods showcase MS Graph sdk and Graph HTTPClient usage with mailbox

            //Get the current timezone setting
            GetUserMailboxDefaultTimeZone();
            //update the timezone setting for the user mailbox
            SetUserMailboxDefaultTimeZone();
            //Get the timezone setting again to verify that its updated
            GetUserMailboxDefaultTimeZone();

            //Showcase method to show how to MS Graph sdk to retrieve messages
            ListUserMailInboxMessages();

            //Create a new message rule
            CreateUserMailBoxRule();
            //Retrieve the message rules to validate
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
        private static void SetUserMailboxDefaultTimeZone()
        {
            const string alias = "admin";
            var mailboxHelper = new MailboxHelper(_graphServiceClient, _httpClient);
            mailboxHelper.SetUserMailboxDefaultTimeZone(alias, "Eastern Standard Time");
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

            var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(authority)
                                                    .WithRedirectUri(redirectUri)
                                                    .WithClientSecret(clientSecret)
                                                    .Build();
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
