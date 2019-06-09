using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkerServiceSample.Utils;

namespace WorkerServiceSample
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //// TEST: infinite loop
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);
            //}

            string pageUrl1 = "https://wakeupandcode.com/unit-testing-in-asp-net-core";
            string pageUrl2 = "https://wakeupandcode.com/validation-in-asp-net-core";

            _logger.LogInformation("Making doc 1 at: {time}", DateTimeOffset.Now);
            DocMaker.MakeDoc(pageUrl1);


            _logger.LogInformation("Making doc 2 at: {time}", DateTimeOffset.Now);
            DocMaker.MakeDoc(pageUrl2);

        }
    }
}
