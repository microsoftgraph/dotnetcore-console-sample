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
            const string alias = "sdk_test";
            const string domain = "gavinbignite.com";
            var userToAdd = BuildUserToAdd("SDK Test User", alias, domain, "ChangeThis!0");
            var added = graphClient.Users.Request().AddAsync(userToAdd).Result;
            Console.WriteLine("Graph SDK Add Result");
            Console.WriteLine(added.DisplayName);

            
            List<QueryOption> queryOptions = new List<QueryOption>
            {
                new QueryOption("$filter", $@"mailNickname eq '{alias}'")
            };

            var newUserResult = graphClient.Users.Request(queryOptions).GetAsync().Result;
            Console.WriteLine(newUserResult[0].DisplayName);
            Console.WriteLine(newUserResult[0].UserPrincipalName);
        }

        private static User BuildUserToAdd(string displayName, string alias, string domain, string password) 
        {
            var passwordProfile = new PasswordProfile
            {
                Password = password,
                ForceChangePasswordNextSignIn = true
            };
            var user = new User
            {
                DisplayName = displayName,
                UserPrincipalName = $@"{alias}@{domain}",
                MailNickname = alias,
                AccountEnabled = true,
                PasswordProfile = passwordProfile
            };
            return user;
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
