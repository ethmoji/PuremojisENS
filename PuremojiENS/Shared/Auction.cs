using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuremojiENS.Shared
{
    public class Auction
    {
        public string Codes { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public string TokenId { get; set; }
        public DateTime? AuctionEnd { get; set; }
        public string LastSale { get; set; }
    }
}
