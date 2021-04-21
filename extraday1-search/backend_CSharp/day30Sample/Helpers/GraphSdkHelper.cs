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
        // Suggested way to authenticate
        public static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            return new GraphServiceClient(authenticationProvider);
        }

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


        private static IAuthenticationProvider CreateAuthorizationProvider(IConfigurationRoot config)
        {
            var clientId = config["applicationId"];
            var redirectUri = config["redirectUri"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}";

            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var pca = PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(authority)
                                                    .WithRedirectUri(redirectUri)
                                                    .Build();
            return new DeviceCodeFlowAuthorizationProvider(pca, scopes);
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
                    string.IsNullOrEmpty(config["domain"]) ||
                    string.IsNullOrEmpty(config["queryKeyword"]))
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
