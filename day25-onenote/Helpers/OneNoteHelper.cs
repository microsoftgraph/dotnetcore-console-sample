using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace ConsoleGraphTest
{
    public class OneNoteHelper
    {

        private GraphServiceClient _graphClient;
        private HttpClient _httpClient;
        public OneNoteHelper(GraphServiceClient graphClient)
        {
            if (null == graphClient) throw new ArgumentNullException(nameof(graphClient));
            _graphClient = graphClient;
        }

        public OneNoteHelper(HttpClient httpClient)
        {
            if (null == httpClient) throw new ArgumentNullException(nameof(httpClient));
            _httpClient = httpClient;
        }

        // Add Public methods to provide functionality for your scenario.

        public async Task CreateNoteBook(string notebookName)
        {
            
        }

        // Add private methods to encapsulate housekeeping work away from public methods

        // private static User BuildUserToAdd(string displayName, string alias, string domain, string password)
        // {

        // }
    }
}