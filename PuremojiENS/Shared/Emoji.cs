using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PuremojiENS.Shared
{
    public class Emoji
    {
        public string Codes { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Subgroup { get; set; }
        public int Width { get; set; }
        [Key]
        public string TokenId { get; set; }
        public string TokenIdHex { get; set; }
        public bool Registered { get; set; }
        public bool ContainsFE0F { get; set; }
        public string LastSale { get; set; }
        public string LastBid { get; set; }
        public DateTime? AuctionEnd { get; set; }
    }
}
