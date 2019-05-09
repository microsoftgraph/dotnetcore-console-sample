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

            GraphServiceClient graphClient = GetAuthenticatedGraphClient(config);

            //Executes the scenario that shows how to add user to unified group
            //Validate the user permissions to the group which also implies the associated SPO site
            PermissionHelperExampleScenario();
        }
        
        private static void PermissionHelperExampleScenario()
        {
            const string alias = "adelev";
            ListUnifiedGroupsForUser(alias);
            string groupId = GetUnifiedGroupStartswith("Contoso");
            AddUserToUnifiedGroup(alias, groupId);
            ListUnifiedGroupsForUser(alias);
        }

        private static void ListUnifiedGroupsForUser(string alias)
        {
            var permissionHelper = new PermissionHelper(_graphServiceClient);
            List<ResultsItem> items = permissionHelper.UserMemberOf(alias).Result;
            Console.WriteLine("User is member of "+ items.Count +" group(s).");
            foreach(ResultsItem item in items)
            {
                Console.WriteLine("  Group Name: "+ item.Display);
            }
        }

        private static string GetUnifiedGroupStartswith(string groupPrefix)
        {
            var permissionHelper = new PermissionHelper(_graphServiceClient);
            var groupId = permissionHelper.GetGroupByName(groupPrefix).Result;
            return groupId;
        }
        private static void AddUserToUnifiedGroup(string alias, string groupId)
        {
            var permissionHelper = new PermissionHelper(_graphServiceClient);
            permissionHelper.AddUserToGroup(alias, groupId).GetAwaiter().GetResult();
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
