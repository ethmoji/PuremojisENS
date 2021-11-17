using System.Net;
using Newtonsoft.Json;
using PuremojiENS.Server.APIs.OpenSea.Models;

namespace PuremojiENS.Server.APIs.OpenSea
{
    public class OpenSea
    {
        private readonly ILogger<OpenSea> _logger;
        private readonly Config _config;
        private readonly HttpClient _httpClient;
        private readonly HttpRequestMessage _registrationRequest;

        public OpenSea(ILogger<OpenSea> logger, Config config)
        {
            _logger = logger;
            _config = config;
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.All;
            _httpClient = new HttpClient(handler);
        }

        //public async Task<GetAssetResponse> GetAsset(string tokenId)
        //{
        //    for (int i = 0; i < _config.APIs.TheGraph.Retries; i++)
        //    {
        //        var request = new HttpRequestMessage(new HttpMethod("GET"),
        //            $"{_config.APIs.OpenSea.GetAssetEndpoint}{tokenId}");

        //        var response = await _httpClient.SendAsync(request);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var jsonString = await response.Content.ReadAsStringAsync();
        //            var asset = JsonConvert.DeserializeObject<GetAssetResponse>(jsonString);
        //            Thread.Sleep(10000);
        //            return asset;
        //        }
        //        Thread.Sleep(1000);
        //        _logger.LogInformation("Trying again");
        //    }

        //    return null;
        //}

        public async Task<GetAssetsResponse> GetAssets(List<string> tokenId)
        {
            var url = $"{_config.APIs.OpenSea.GetAssetsEndpoint}&{string.Join("&", tokenId.Select(x => $"token_ids={x}"))}";
            var request = new HttpRequestMessage(new HttpMethod("GET"), url);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var assets = JsonConvert.DeserializeObject<GetAssetsResponse>(jsonString);
                return assets;
            }

            return null;
        }

        public async Task<EventsResponse> GetEvents(int minutesBack)
        {
            var url = $"{_config.APIs.OpenSea.EventsEndpoint}&occurred_after={DateTimeOffset.Now.AddMinutes(-minutesBack).ToUnixTimeSeconds()}";
            var request = new HttpRequestMessage(new HttpMethod("GET"), url);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var events = JsonConvert.DeserializeObject<EventsResponse>(jsonString);
                return events;
            }

            return null;
        }
    }
}