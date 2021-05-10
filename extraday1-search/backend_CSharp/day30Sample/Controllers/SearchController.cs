using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Graph;
using day30Sample.helpers;
using day30Sample.Model;
using Microsoft.Extensions.Primitives;

namespace day30Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;

        public SearchController(ILogger<SearchController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<SearchResponse> PostAsync(SearchRequest searchRequest)
        {
            Request.Headers.TryGetValue("Custom-Token", out StringValues token);
            SearchResponse response = await SearchHelper.Search(searchRequest, token.ToString());
            return response;
        }

    }
}
