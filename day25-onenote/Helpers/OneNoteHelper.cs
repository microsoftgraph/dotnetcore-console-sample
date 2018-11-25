using System;
using System.Collections.Generic;
using System.IO;
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

        public Notebook GetNotebook(string upn, string notebookName)
        {
            List<QueryOption> options = new List<QueryOption>
            {
                new QueryOption("$filter", $"DisplayName eq '{notebookName}'")
            };
            var notebooks = (_graphClient.Users[upn].Onenote.Notebooks.Request(options).GetAsync()).Result;

            if(notebooks.Count > 0)
            {
                return notebooks[0];
            }
            else
            {
                return null;
            }
        }

        public async Task<Notebook> CreateNoteBook(string upn, string notebookName)
        {
            var notebook = new Notebook{
                DisplayName = notebookName
            };

            return (await _graphClient.Users[upn].Onenote.Notebooks.Request().AddAsync(notebook));
        }

        public OnenoteSection GetSection(string upn, Notebook notebook, string sectionName)
        {
            List<QueryOption> options = new List<QueryOption>
            {
                new QueryOption("$filter", $"DisplayName eq '{sectionName}'")
            };

            var sections = (_graphClient.Users[upn].Onenote.Notebooks[notebook.Id].Sections.Request(options).GetAsync()).Result;

            if(sections.Count > 0)
            {
                return sections[0];
            }
            else
            {
                return null;
            }
        }

        public async Task<OnenoteSection> CreateSection(string upn, Notebook notebook, string sectionName)
        {
            var section = new OnenoteSection{
                DisplayName = sectionName
            };

            return (await _graphClient.Users[upn].Onenote.Notebooks[notebook.Id].Sections.Request().AddAsync(section));
        }

        public OnenotePage GetPage(string upn, OnenoteSection section, string pageName)
        {
            List<QueryOption> options = new List<QueryOption>
            {
                new QueryOption("$filter", $"Title eq '{pageName}'")
            };

            var pages = (_graphClient.Users[upn].Onenote.Sections[section.Id].Pages.Request(options).GetAsync()).Result;

            if(pages.Count > 0)
            {
                return pages[0];
            }
            else
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> CreatePage(string upn, OnenoteSection section, string pageName)
        {
             Uri Uri = new Uri($"https://graph.microsoft.com/v1.0/users/{upn}/onenote/sections/{section.Id}/pages");

            // use a verbatim interpolated string to represetnt the HTML text to be used for page creation
            var html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>{pageName}</title>
            </head>
            <body>
                I'm learning about the Microsoft Graph!
            </body>
            </html>";

            HttpContent httpContent = new StringContent(html, System.Text.Encoding.UTF8, "application/xhtml+xml");

            return (await _httpClient.PostAsync(Uri, httpContent));
        }
    }
}