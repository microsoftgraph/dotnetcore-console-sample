using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace ConsoleGraphTest
{
    public class MailboxHelper
    {
        private GraphServiceClient _graphClient;
        private HttpClient _httpClient;
        
        public MailboxHelper(GraphServiceClient graphClient)
        {
            if (null == graphClient) throw new ArgumentNullException(nameof(graphClient));
                _graphClient = graphClient;
        }

        public MailboxHelper(HttpClient httpClient)
        {
            if (null == httpClient) throw new ArgumentNullException(nameof(httpClient));
                _httpClient = httpClient;
        }

        public async Task<List<ResultsItem>> ListInboxMessages(string alias)
        {
            User user = FindByAlias(alias).Result;
            List<ResultsItem> items = new List<ResultsItem>();

            IMailFolderMessagesCollectionPage messages = await _graphClient.Users[user.Id].MailFolders.Inbox.Messages.Request().Top(10).GetAsync();
            if (messages?.Count > 0)
            {
                foreach (Message message in messages)
                {
                    items.Add(new ResultsItem
                    {
                        Display = message.Subject,
                        Id = message.Id
                    });
                }
            }
            return items;
        }
        public async Task<string> GetUserMailboxDefaultTimeZone(string alias)
        {
            User user = FindByAlias(alias).Result;
            User detailedUser = await _graphClient.Users[user.Id].Request().Select("MailboxSettings").GetAsync();
            return detailedUser.MailboxSettings.TimeZone;
        }
        public async void SetUserMailboxDefaultTimeZone(string alias, string timezone)
        {
            User user = FindByAlias(alias).Result;
            Uri Uri = new Uri("https://graph.microsoft.com/v1.0/users/"+ user.Id +"/mailboxSettings");
            String jsonContent = "{\"timeZone\" : \""+ timezone +"\"}";
            HttpContent httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            await _httpClient.PatchAsync(Uri, httpContent);
        }        
     
        public async Task<List<ResultsItem>> GetUserMailboxRules(string alias)
        {
            User user = FindByAlias(alias).Result;
            IMailFolderMessageRulesCollectionPage rules = await _graphClient.Users[user.Id].MailFolders.Inbox.MessageRules.Request().GetAsync(); 
            List<ResultsItem> items = new List<ResultsItem>();
            if (rules?.Count > 0)
            {
                foreach (MessageRule rule in rules)
                {
                    items.Add(new ResultsItem
                    {
                        Display = rule.DisplayName,
                        Id = rule.Id
                    });
                }
            }
            return items;
        }
        public async Task CreateRule(string alias, string displayName, int sequence, bool isEnabled, string senderContains, string forwardToEmail)
        {
            MessageRule rule = BuildMailRule(displayName, sequence, isEnabled, senderContains, forwardToEmail);
            User user = FindByAlias(alias).Result;
            await _graphClient.Users[user.Id].MailFolders.Inbox.MessageRules.Request().AddAsync(rule);
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
        
        private static MessageRule BuildMailRule(string displayName, int sequence, bool isEnabled, string senderContains, string forwardToEmail) 
        {
            IEnumerable<string> senderContainsList = new string[]{senderContains};
            EmailAddress email = new EmailAddress(){
                Address = forwardToEmail
            };
            Recipient recipient = new Recipient(){
                EmailAddress = email
            };
            IEnumerable<Recipient> recipientList = new Recipient[]{ recipient };
            var msgRule = new MessageRule{
                DisplayName = displayName,
                Sequence = sequence,
                IsEnabled = isEnabled,
                Conditions = new MessageRulePredicates{
                    SenderContains = senderContainsList
                },
                Actions = new MessageRuleActions{
                    ForwardTo = recipientList
                }
            };
            return msgRule;
        }
    }
}