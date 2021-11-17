using Microsoft.AspNetCore.Mvc;
using PuremojiENS.Shared;

namespace PuremojiENS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuremojisController : ControllerBase
    {
        private readonly ILogger<PuremojisController> _logger;
        private readonly List<Emoji> _puremojis;

        public PuremojisController(ILogger<PuremojisController> logger, List<Emoji> puremojis)
        {
            _logger = logger;
            _puremojis = puremojis;
        }

        [HttpGet]
        public IEnumerable<Emoji> Get(string? filter, string? status)
        {
            _logger.LogInformation("Puremojis requested");
            
            
            // Redundant switch cases to save temp memory
            switch (filter)
            {
                case "triples":
                    switch(status)
                    {
                        case "registered":
                            return _puremojis.Where(x => x.Registered && x.Width == 1);
                        case "unregistered":
                            return _puremojis.Where(x => !x.Registered && x.Width == 1);
                        default:
                            return _puremojis.Where(x => x.Width == 1);
                    }
                case "doubles":
                    switch (status)
                    {
                        case "registered":
                            return _puremojis.Where(x => x.Registered && x.Width == 2);
                        case "unregistered":
                            return _puremojis.Where(x => !x.Registered && x.Width == 2);
                        default:
                            return _puremojis.Where(x => x.Width == 2);
                    }
                case "singles":
                    switch (status)
                    {
                        case "registered":
                            return _puremojis.Where(x => x.Registered && x.Width >= 3);
                        case "unregistered":
                            return _puremojis.Where(x => !x.Registered && x.Width >= 3);
                        default:
                            return _puremojis.Where(x => x.Width >= 3);
                    }
                default:
                    return _puremojis;
            }
        }
    }
}