using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuremojiENS.Server.APIs.OpenSea;
using PuremojiENS.Server.APIs.TheGraph;
using PuremojiENS.Server.Directory;
using PuremojiENS.Shared;

namespace PuremojiENS.Server.Worker
{
    public class UpdateOpenSeaData : IHostedService, IDisposable
    {
        private readonly ILogger<UpdateOpenSeaData> _logger;
        private readonly EmojisDbContext _dbContext;
        private readonly Config _config;
        private readonly TheGraph _theGraph;
        private readonly OpenSea _openSea;
        private Timer _timer = null!;

        public UpdateOpenSeaData(ILogger<UpdateOpenSeaData> logger, EmojisDbContext dbContext, Config config, TheGraph theGraph, OpenSea openSea)
        {
            _logger = logger;
            _dbContext = dbContext;
            _config = config;
            _theGraph = theGraph;
            _openSea = openSea;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpdateOpenSeaData Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_config.UpdateCooldowns.OpenSeaData));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation("Updating OpenSea data");
            var puremojis = _dbContext.Emojis.Where(x => !x.ContainsFE0F);
            var puremojisCount = puremojis.Count();


            for (int i = 0; i < puremojisCount; i += 30)
            {
                var elements = puremojisCount - i >= 30 ? 30 : puremojisCount - i;
                _logger.LogInformation($"[{i},{i + elements}]");
                var next = puremojis.Skip(i).Take(elements);
                var assets = await _openSea.GetAssets(next.Select(x => x.TokenId).ToList());
                if (assets != null)
                {
                    foreach (var asset in assets.assets)
                    {
                        if (asset != null)
                        {
                            var curr = puremojis.First(x => x.TokenId.Equals(asset.token_id));
                            if (asset.last_sale != null && !string.IsNullOrEmpty(asset.last_sale.total_price))
                            {
                                curr.LastSale = asset.last_sale.total_price;
                                _logger.LogInformation($"Last Sale of {curr.TokenId}: {curr.LastSale} ETH");
                            }
                            if (asset.sell_orders != null)
                            {
                                var sellOrder = asset.sell_orders.FirstOrDefault();
                                if (sellOrder != null && sellOrder.closing_date != null && sellOrder.closing_extendable)
                                {
                                    curr.AuctionEnd = sellOrder.closing_date;
                                    _logger.LogInformation($"Auction end {curr.AuctionEnd.Value.ToLongDateString()} ETH");
                                }
                                else
                                {
                                    curr.AuctionEnd = null;
                                }
                            }
                            else
                            {
                                curr.AuctionEnd = null;
                            }
                        }
                    }
                }
                if(i % 300 == 0)
                {
                    await _dbContext.SaveChangesAsync();
                }
                Thread.Sleep(10000);
            }

            await _dbContext.SaveChangesAsync();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpdateOpenSeaData is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}