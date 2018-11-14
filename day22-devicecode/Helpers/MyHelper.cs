using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace ConsoleGraphTest
{
    /**
     * Please rename your helper class to match it's purpose
     */
    public class MyHelper
    {

        /**
         * Inject either a GraphServiceClient or an HttpClient (with Authentiation supplied)
         * Which you choose to use will depend on your scenario but the GraphServiceClient should be used where practical
         * Please delete the constructor you don't use
         */
        private GraphServiceClient _graphClient;
        private HttpClient _httpClient;
        public MyHelper(GraphServiceClient graphClient)
        {
            if (null == graphClient) throw new ArgumentNullException(nameof(graphClient));
            _graphClient = graphClient;
        }

        public MyHelper(HttpClient httpClient)
        {
            if (null == httpClient) throw new ArgumentNullException(nameof(httpClient));
            _httpClient = httpClient;
        }

        // Add Public methods to provide functionality for your scenario.

        public async Task<User> FindByAlias(string alias)
        {
            List<QueryOption> queryOptions = new List<QueryOption>
            {
                new QueryOption("$filter", $@"mailNickname eq '{alias}'")
            };

            var userResult = await _graphClient.Users.Request(queryOptions).GetAsync();
            if (userResult.Count != 1) throw new ApplicationException($"Unable to find a user with the alias {alias}");
            return userResult[0];
        }

        // Add private methods to encapsulate housekeeping work away from public methods

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
    }
}