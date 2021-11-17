using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuremojiENS.Server.APIs.TheGraph.Models;

namespace PuremojiENS.Server.APIs.TheGraph
{
    public class TheGraph
    {
        private readonly ILogger<TheGraph> _logger;
        private readonly Config _config;
        private readonly HttpClient _httpClient;
        private readonly HttpRequestMessage _registrationRequest;

        public TheGraph(ILogger<TheGraph> logger, Config config)
        {
            _logger = logger;
            _config = config;
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.All;
            _httpClient = new HttpClient(handler);
            _registrationRequest = new HttpRequestMessage(new HttpMethod("POST"),
                "https://api.thegraph.com/subgraphs/name/ensdomains/ens");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authority", "api.thegraph.com");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "*/*");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-gpc", "1");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("origin", "https://app.ens.domains");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-site", "cross-site");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-mode", "cors");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-dest", "empty");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("referer", "https://app.ens.domains/");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");
        }

        public async Task<bool> IsRegistered(string tokenHex)
        {
            for (int i = 0; i < _config.APIs.TheGraph.Retries; i++)
            {
                var request = new HttpRequestMessage(new HttpMethod("POST"),
                    _config.APIs.TheGraph.EnsEndpoint);
                var body = _config.APIs.TheGraph.Query.Replace("{tokenHex}", tokenHex);
                request.Content = new StringContent(body);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var registration = JsonConvert.DeserializeObject<RegistrationResponse>(await response.Content.ReadAsStringAsync());
                    return registration.data.registration != null;
                }
                Thread.Sleep(500);
            }

            _logger.LogError("Could not successfully query {0}", tokenHex);
            return false;
        }
    }
}