using Microsoft.AspNetCore.Mvc;
using PuremojiENS.Shared;

namespace PuremojiENS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidTokenIdsController : ControllerBase
    {
        private readonly ILogger<ValidTokenIdsController> _logger;
        private readonly List<string> _validTokenIds;

        public ValidTokenIdsController(ILogger<ValidTokenIdsController> logger, List<string> validTokenIds)
        {
            _logger = logger;
            _validTokenIds = validTokenIds;
        }

        [HttpGet]
        public IEnumerable<string?> Get()
        {
            _logger.LogInformation("Valid TokenIds requested");

            return _validTokenIds;
        }
    }
}