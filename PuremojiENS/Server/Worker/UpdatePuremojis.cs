using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuremojiENS.Server.APIs.OpenSea;
using PuremojiENS.Server.APIs.TheGraph;
using PuremojiENS.Shared;

namespace PuremojiENS.Server.Worker
{
    public class UpdatePuremojis : IHostedService, IDisposable
    {
        private readonly ILogger<UpdatePuremojis> _logger;
        private readonly Config _config;
        private readonly TheGraph _theGraph;
        private readonly List<Emoji> _puremojis;
        private readonly OpenSea _openSea;
        private Timer _timer = null!;

        public UpdatePuremojis(ILogger<UpdatePuremojis> logger, Config config, TheGraph theGraph, List<Emoji> puremoji, OpenSea openSea)
        {
            _logger = logger;
            _config = config;
            _theGraph = theGraph;
            _puremojis = puremoji;
            _openSea = openSea;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpdatePureTriples Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_config.UpdateCooldown));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation("Updating puremojis");

            foreach (var emoji in _puremojis)
            {
                if (!emoji.Registered)
                {
                    _logger.LogInformation(emoji.Name);
                    var registered = await _theGraph.IsRegistered(emoji.TokenIdHex);
                    emoji.Registered = registered;
                }
            }
            _logger.LogInformation("Done updating registration");

            for (int i = 0; i < _puremojis.Count; i += 30)
            {
                var elements = _puremojis.Count - i >= 30 ? 30 : _puremojis.Count - i;
                _logger.LogInformation($"[{i},{i + elements}]");
                var next = _puremojis.GetRange(i, elements);
                var assets = await _openSea.GetAssets(next.Select(x => x.TokenId).ToList());
                foreach (var asset in assets.assets)
                {
                    if (asset != null)
                    {
                        if (asset.last_sale != null && !string.IsNullOrEmpty(asset.last_sale.total_price))
                        {
                            foreach (var n in next)
                            {
                                if (n.TokenId.Equals(asset.token_id))
                                {
                                    n.LastSale = decimal.Parse(asset.last_sale.total_price) / _config.EthDivisor;
                                    _logger.LogInformation($"Last Sale of {n.TokenId}: {n.LastSale} ETH");
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(10000);
            }

            _logger.LogInformation("Done updating last sales");

            var json = JsonConvert.SerializeObject(_puremojis);
            await File.WriteAllTextAsync(_config.PuremojisPath, json, Encoding.UTF8);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpdatePureTriplesService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}