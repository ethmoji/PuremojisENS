namespace PuremojiENS.Shared
{
    public class Emoji
    {
        public string? Codes { get; set; }
        public string? Name { get; set; }
        public string? Group { get; set; }
        public string? Subgroup { get; set; }
        public int Width { get; set; }
        public string? TokenId { get; set; }
        public string? TokenIdHex { get; set; }
        public bool Registered { get; set; }
        public decimal LastSale { get; set; }
    }
}
