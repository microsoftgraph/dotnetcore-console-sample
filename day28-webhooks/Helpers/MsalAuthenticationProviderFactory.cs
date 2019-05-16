
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace GraphWebhooks
{
    // This class encapsulates the details of getting a token from MSAL and exposes it via the
    // IAuthenticationProvider interface so that GraphServiceClient or AuthHandler can use it.
    // A significantly enhanced version of this class will in the future be available from
    // the GraphSDK team.  It will supports all the types of Client Application as defined by MSAL.
    public class GraphHttpClientFactory
    {
        public static HttpClient GetAuthenticatedHTTPClient(IConfiguration config)
        {
            var authenticationProvider = BuildAuthProvider(config);
            return new HttpClient(new AuthHandler(authenticationProvider, new HttpClientHandler()));
        }

        private static IAuthenticationProvider BuildAuthProvider(IConfiguration config)
        {
            var clientId = config["applicationId"];
            var clientSecret = config["applicationSecret"];
            var redirectUri = config["redirectUri"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}/v2.0";

            //this specific scope means that application will default to what is defined in the application registration rather than using dynamic scopes
            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(authority)
                                                    .WithRedirectUri(redirectUri)
                                                    .WithClientSecret(clientSecret)
                                                    .Build();

            return new MsalAuthenticationProvider(cca, scopes.ToArray());
        }
    }
}