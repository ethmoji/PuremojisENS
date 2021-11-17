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
    public class UpdatePuremojis : IHostedService, IDisposable
    {
        private readonly ILogger<UpdatePuremojis> _logger;
        private readonly EmojisDbContext _dbContext;
        private readonly Config _config;
        private readonly TheGraph _theGraph;
        private readonly OpenSea _openSea;
        private Timer _timer = null!;

        public UpdatePuremojis(ILogger<UpdatePuremojis> logger, EmojisDbContext dbContext, Config config, TheGraph theGraph, OpenSea openSea)
        {
            _logger = logger;
            _dbContext = dbContext;
            _config = config;
            _theGraph = theGraph;
            _openSea = openSea;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpdatePureTriples Service running.");

            foreach(var emoji in _dbContext.Emojis)
            {
                if(DateTime.Now > emoji.AuctionEnd)
                {
                    emoji.AuctionEnd = null;
                }
            }
            
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_config.UpdateCooldown));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            var events = await _openSea.GetEvents(_config.UpdateCooldown / 60);
            if(events != null)
            {
                foreach (var e in events.asset_events)
                {
                    if(e.asset != null && e.asset.token_id != null)
                    {
                        var emoji = _dbContext.Emojis.FirstOrDefault(x => x.TokenId == e.asset.token_id);
                        if (emoji != null)
                        {
                            if (e.event_type == "created" && e.auction_type != null && e.auction_type== "english")
                            {
                                emoji.AuctionEnd = e.created_date.AddSeconds(int.Parse(e.duration));
                                _logger.LogInformation($"Created for {e.asset.permalink}");
                            }
                            else if (e.event_type == "cancelled")
                            {
                                emoji.AuctionEnd = null;
                                _logger.LogInformation($"Ended for {e.asset.permalink}");
                            }
                            else if (e.event_type == "successful")
                            {
                                emoji.LastSale = e.total_price;
                                _logger.LogInformation($"Sale for {e.asset.permalink}");
                            }
                        }
                    }
                }
            }

            //var puremojis = _dbContext.Emojis.Where(x => !x.ContainsFE0F);
            //var puremojisCount = puremojis.Count();

            //foreach (var emoji in puremojis)
            //{
            //    if (!emoji.Registered)
            //    {
            //        _logger.LogInformation(emoji.Name);
            //        var registered = await _theGraph.IsRegistered(emoji.TokenIdHex);
            //        emoji.Registered = registered;
            //    }
            //}
            //_logger.LogInformation("Done updating registration");


            //for (int i = 0; i < puremojisCount; i += 30)
            //{
            //    var elements = puremojisCount - i >= 30 ? 30 : puremojisCount - i;
            //    _logger.LogInformation($"[{i},{i + elements}]");
            //    var next = puremojis.Take(elements);
            //    var assets = await _openSea.GetAssets(next.Select(x => x.TokenId).ToList());
            //    if (assets != null)
            //    {
            //        foreach (var asset in assets.assets)
            //        {
            //            if (asset != null)
            //            {
            //                if (asset.last_sale != null && !string.IsNullOrEmpty(asset.last_sale.total_price))
            //                {
            //                    foreach (var n in next)
            //                    {
            //                        if (n.TokenId.Equals(asset.token_id))
            //                        {
            //                            n.LastSale = asset.last_sale.total_price;
            //                            _logger.LogInformation($"Last Sale of {n.TokenId}: {n.LastSale} ETH");
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    Thread.Sleep(10000);
            //}


            //_dbContext.Emojis.UpdateRange(puremojis);
            await _dbContext.SaveChangesAsync();
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