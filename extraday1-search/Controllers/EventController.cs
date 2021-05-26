using day30Sample.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Graph;
using System.Threading.Tasks;

namespace day30Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;

        public EventController(ILogger<EventController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<Event> GetAsync(string eventId)
        {
            eventId = eventId.Trim().Replace(" ", "+");
            Request.Headers.TryGetValue("Custom-Token", out StringValues token);
            return await EventHelper.GetEventByEventId(eventId, token.ToString());
        }
    }
}
