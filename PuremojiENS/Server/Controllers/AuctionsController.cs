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
        private readonly EmojisDbContext _dbContext;

        public AuctionsController(ILogger<ValidTokenIdsController> logger, EmojisDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Auction> Get()
        {
            _logger.LogInformation("Auctions requested");

            return _dbContext.Emojis
                .Where(x => x.AuctionEnd != null)
                .Select(x => new Auction() { AuctionEnd = x.AuctionEnd, Name = x.Name, Codes = x.Codes, TokenId = x.TokenId, Width = x.Width, LastSale = x.LastSale})
                .OrderBy(x => x.AuctionEnd);
        }
    }
}