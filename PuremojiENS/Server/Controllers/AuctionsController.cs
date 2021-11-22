using Microsoft.AspNetCore.Mvc;
using PuremojiENS.Server.Directory;
using PuremojiENS.Shared;

namespace PuremojiENS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly ILogger<ValidTokenIdsController> _logger;
        private readonly Config _config;
        private readonly EmojisDbContext _dbContext;

        public AuctionsController(ILogger<ValidTokenIdsController> logger, Config config, EmojisDbContext dbContext)
        {
            _logger = logger;
            _config = config;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Auction> Get()
        {
            _logger.LogInformation("Auctions requested");

            return _dbContext.Emojis
                .Where(x => x.AuctionEnd != null && x.Width == 1 && x.AuctionEnd <= DateTime.Now.AddHours(_config.AuctionHoursInTheFuture))
                .Select(x => new Auction() { AuctionEnd = x.AuctionEnd, Name = x.Name, Codes = x.Codes, TokenId = x.TokenId, Width = x.Width, LastSale = x.LastSale });
        }
    }
}