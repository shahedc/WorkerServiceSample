using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MariGold.OpenXHTML;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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


            // Get HTML from website
            string htmlContent = string.Empty;
            string pageUrl = "https://wakeupandcode.com/key-vault-for-asp-net-core-web-apps";
            string outputFileName = "_output.docx";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pageUrl);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                htmlContent = reader.ReadToEnd();

            }
            outputFileName = request.RequestUri.AbsolutePath.Substring(1);

            //Console.WriteLine(html);


            // Get content
            HtmlDocument htmlDoc = new HtmlDocument();
            //htmlDoc.Load(@"file.htm");
            htmlDoc.LoadHtml(htmlContent);

            HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]");
            string nodeContent = (node == null) ? "Error, id not found" : node.InnerHtml;


            Console.WriteLine("Making your ebook...");

            // Create DOCX file
            WordDocument wordDoc = new WordDocument($"{outputFileName}.docx");
            wordDoc.Process(new HtmlParser(nodeContent));
            wordDoc.Save();

            //WordDocument doc = new WordDocument("blog.docx");
            //doc.BaseURL = "http:\\wakeupandcode.com";
            //doc.Process(new HtmlParser("<a href=\"index.htm\">sample</a>"));
            //doc.Save();

        }
    }
}
