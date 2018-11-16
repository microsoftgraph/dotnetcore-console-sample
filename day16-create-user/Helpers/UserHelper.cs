using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace ConsoleGraphTest
{
    public class UserHelper
    {
        private GraphServiceClient _graphClient;
        public UserHelper(GraphServiceClient graphClient)
        {
            if (null == graphClient) throw new ArgumentNullException(nameof(graphClient));
            _graphClient = graphClient;
        }

        public async Task CreateUser(string displayName, string alias, string domain, string password)
        {
            var userToAdd = BuildUserToAdd(displayName, alias, domain, password);
            await _graphClient.Users.Request().AddAsync(userToAdd);
        }

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