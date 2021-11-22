using Microsoft.AspNetCore.Mvc;
using PuremojiENS.Server.Directory;
using PuremojiENS.Shared;

namespace PuremojiENS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuremojisController : ControllerBase
    {
        private readonly ILogger<PuremojisController> _logger;
        private readonly EmojisDbContext _dbContext;

        public PuremojisController(ILogger<PuremojisController> logger, EmojisDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Emoji> Get(string? filter)
        {
            _logger.LogInformation("Puremojis requested");
            
            
            switch (filter)
            {
                case "triples":
                    return _dbContext.Emojis.Where(x => !x.ContainsFE0F && x.Width == 1);
                case "doubles":
                    return _dbContext.Emojis.Where(x => !x.ContainsFE0F && x.Width == 2);
                case "singles":
                    return _dbContext.Emojis.Where(x => !x.ContainsFE0F && x.Width >= 3);
                default:
                    return _dbContext.Emojis.Where(x => !x.ContainsFE0F);
            }
        }
    }
}