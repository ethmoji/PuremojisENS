namespace PuremojiENS.Client
{
    public class Config
    {
        public COpenSea OpenSea { get; set; }
        public CENS ENS { get; set; }
        public decimal EthDivisor { get; set; }
    }
    
    public class COpenSea
    {
        public string ENSUrl { get; set; }
    }

    public class CENS
    {
        public string RegisterUrl { get; set; }
    }

    public class CApi
    {
        public string PuremojisEndpoint { get; set; }
        public string ValidTokenIdsEndpoint { get; set; }
        public string AuctionsEndpoint { get; set; }
    }
}
