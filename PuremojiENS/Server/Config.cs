namespace PuremojiENS.Server
{
    public class Config
    {
        public string AllowedHosts { get; set; }
        public string PuremojisPath { get; set; }
        public string ValidTokenIdsPath { get; set; }
        public decimal EthDivisor { get; set; }
        public int AuctionHoursInTheFuture { get; set; }
        public CAPIs APIs { get; set; }
        public CUpdateCooldowns UpdateCooldowns{ get; set; }
    }

    public class CAPIs
    {
        public CTheGraph TheGraph { get; set; }
        public COpenSea OpenSea { get; set; }
    }

    public class CUpdateCooldowns
    {
        public int Registrations { get; set; }
        public int OpenSeaData { get; set; }
    }

    public class CTheGraph
    {
        public string EnsEndpoint { get; set; }
        public string Query { get; set; }
        public int Retries { get; set; }
    }

    public class COpenSea
    {
        public string GetAssetEndpoint { get; set; }
        public string GetAssetsEndpoint { get; set; }
        public string EventsEndpoint { get; set; }
    }
}
