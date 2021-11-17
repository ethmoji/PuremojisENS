using System.Collections.Generic;

namespace PuremojiENS.Server.APIs.OpenSea.Models
{
    public class GetAssetsResponse
    {
        public List<GetAssetResponse> assets { get; set; }
    }
}