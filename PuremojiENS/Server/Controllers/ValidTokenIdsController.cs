using Microsoft.AspNetCore.Mvc;
using PuremojiENS.Server.Directory;
using PuremojiENS.Shared;

namespace PuremojiENS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidTokenIdsController : ControllerBase
    {
        private readonly ILogger<ValidTokenIdsController> _logger;
        private readonly EmojisDbContext _dbContext;

        public ValidTokenIdsController(ILogger<ValidTokenIdsController> logger, EmojisDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInformation("Valid TokenIds requested");

            return _dbContext.Emojis
                .Select(x => x.TokenId);
        }
    }
}