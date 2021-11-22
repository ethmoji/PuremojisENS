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
    public class UpdateRegistrations : IHostedService, IDisposable
    {
        private readonly ILogger<UpdateRegistrations> _logger;
        private readonly EmojisDbContext _dbContext;
        private readonly Config _config;
        private readonly TheGraph _theGraph;
        private readonly OpenSea _openSea;
        private Timer _timer = null!;

        public UpdateRegistrations(ILogger<UpdateRegistrations> logger, EmojisDbContext dbContext, Config config, TheGraph theGraph, OpenSea openSea)
        {
            _logger = logger;
            _dbContext = dbContext;
            _config = config;
            _theGraph = theGraph;
            _openSea = openSea;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpdateRegistrations Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_config.UpdateCooldowns.Registrations));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation("Updating registration");
            var puremojis = _dbContext.Emojis.Where(x => !x.ContainsFE0F);
            var puremojisCount = puremojis.Count();

            foreach (var emoji in puremojis)
            {
                if (!emoji.Registered)
                {
                    var registered = await _theGraph.IsRegistered(emoji.TokenIdHex);
                    emoji.Registered = registered;
                    if (registered)
                        _logger.LogInformation($"Now registered {emoji.Name}");
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Done updating registrations");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpdateRegistrations is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}