using System.Collections.Generic;

namespace PuremojiENS.Server.APIs.OpenSea.Models
{
    public class GetAssetResponse
    {
        public string token_id { get; set; }
        public LastSale last_sale { get; set; }
    }

    public class LastSale
    {
        public string total_price { get; set; }
    }
}