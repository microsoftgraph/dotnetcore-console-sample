using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace ConsoleGraphTest
{
    public class SearchHelper
    {
        private GraphServiceClient _graphClient;
        public SearchHelper(GraphServiceClient graphClient)
        {
            if (null == graphClient) throw new ArgumentNullException(nameof(graphClient));
            _graphClient = graphClient;
        }

        //search message
        public async Task<ISearchEntityQueryCollectionPage> SearchMessage(string keyword)
        {
            List<SearchRequestObject> sro = new List<SearchRequestObject>
            {
                new SearchRequestObject{
                    EntityTypes = new List<EntityType>
                    {
                        EntityType.Message
                    },
                    Query = new SearchQuery{
                        QueryString = keyword
                    },
                    From = 0,
                    Size = 25
                }
            };

            var messageResult = await _graphClient.Search.Query(sro).Request().PostAsync();
            if (messageResult.Count == 0) throw new ApplicationException($"Unable to find a message with the keyword {keyword}");

            return messageResult;
        }

        //search event
        public async Task<ISearchEntityQueryCollectionPage> SearchEvent(string keyword)
        {
            List<SearchRequestObject> sro = new List<SearchRequestObject>
            {
                new SearchRequestObject{
                    EntityTypes = new List<EntityType>
                    {
                        EntityType.Event
                    },
                    Query = new SearchQuery{
                        QueryString = keyword
                    },
                    From = 0,
                    Size = 25
                }
            };

            var messageResult = await _graphClient.Search.Query(sro).Request().PostAsync();
            if (messageResult.Count == 0) throw new ApplicationException($"Unable to find a event with the keyword {keyword}");

            return messageResult;
        }

        //search site
        public async Task<ISearchEntityQueryCollectionPage> SearchSite(string keyword)
        {
            List<SearchRequestObject> sro = new List<SearchRequestObject>
            {
                new SearchRequestObject{
                    EntityTypes = new List<EntityType>
                    {
                        EntityType.Site
                    },
                    Query = new SearchQuery{
                        QueryString = keyword
                    },
                    From = 0,
                    Size = 25
                }
            };

            var messageResult = await _graphClient.Search.Query(sro).Request().PostAsync();
            if (messageResult.Count == 0) throw new ApplicationException($"Unable to find a site with the keyword {keyword}");

            return messageResult;
        }
        //search driveItem
        public async Task<ISearchEntityQueryCollectionPage> SearchDriveItem(string keyword)
        {
            List<SearchRequestObject> sro = new List<SearchRequestObject>
            {
                new SearchRequestObject{
                    EntityTypes = new List<EntityType>
                    {
                        EntityType.DriveItem
                    },
                    Query = new SearchQuery{
                        QueryString = keyword
                    },
                    From = 0,
                    Size = 25
                }
            };

            var messageResult = await _graphClient.Search.Query(sro).Request().PostAsync();
            if (messageResult.Count == 0) throw new ApplicationException($"Unable to find a driveItem with the keyword {keyword}");

            return messageResult;
        }
    }
}