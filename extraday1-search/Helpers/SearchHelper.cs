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

        public async Task<ISearchEntityQueryCollectionPage> SearchEntityByKeyword(string queryKeyword, EntityType queryEntityType, int? from = 0, int? size = 25)
        {
            List<SearchRequestObject> sro = new List<SearchRequestObject>
            {
                new SearchRequestObject{
                    EntityTypes = new List<EntityType>
                    {
                        queryEntityType
                    },
                    Query = new SearchQuery{
                        QueryString = queryKeyword
                    },
                    From = from,
                    Size = size
                }
            };

            var queryResult = await _graphClient.Search.Query(sro).Request().PostAsync();
            if (queryResult.Count == 0) throw new ApplicationException($"Unable to find a {queryEntityType.ToString()} with the keyword {queryKeyword}");

            return queryResult;
        }
    }
}