using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace ConsoleGraphTest
{
    public class PermissionHelper
    {
        private GraphServiceClient _graphClient;
        private HttpClient _httpClient;
        public PermissionHelper(GraphServiceClient graphClient)
        {
            if (null == graphClient) throw new ArgumentNullException(nameof(graphClient));
            _graphClient = graphClient;
        }

        public PermissionHelper(HttpClient httpClient)
        {
            if (null == httpClient) throw new ArgumentNullException(nameof(httpClient));
            _httpClient = httpClient;
        }
        
        //Returns a list of groups that the given user belongs to
        public async Task<List<ResultsItem>> UserMemberOf(string alias)
        {
            User user = FindByAlias(alias).Result;
            List<ResultsItem> items = new List<ResultsItem>();

            IUserMemberOfCollectionWithReferencesPage groupsCollection = await _graphClient.Users[user.Id].MemberOf.Request().GetAsync();
            if (groupsCollection?.Count > 0)
            {
                foreach (DirectoryObject dirObject in groupsCollection)
                {
                    if (dirObject is Group)
                    {
                        Group group = dirObject as Group;
                        items.Add(new ResultsItem
                        {
                            Display = group.DisplayName,
                            Id = group.Id
                        });
                    }
                }
            }
            return items;
        }

        //Adds the user to the given group if not already a member of
        public async Task AddUserToGroup(string alias, string groupId)
        {
            User user = FindByAlias(alias).Result;
            List<ResultsItem> items = UserMemberOf(alias).Result;
            if (items.FindIndex(f => f.Id == groupId) >= 0)
                Console.WriteLine("User already belongs to this group");
            else
                await _graphClient.Groups[groupId].Members.References.Request().AddAsync(user);
        }

        //Returns the first unified group with the given suffix
        public async Task<string> GetGroupByName(string groupNameSuffix)
        {
            var groups = await _graphClient.Groups.Request().Filter("groupTypes/any(c:c%20eq%20'unified') AND startswith(displayName,'" + groupNameSuffix + "')").Select("displayName,description,id").GetAsync();
            if (groups?.Count > 0)
            {
                return (groups[0] as Group).Id as string;
            }
            return null;
        }

        //Returns the User object for the given alias
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
    }
}