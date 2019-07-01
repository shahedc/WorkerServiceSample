using HtmlAgilityPack;
using MariGold.OpenXHTML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace WorkerServiceSample.Utils
{
    public class DocMaker
    {
        public static void MakeDoc(string pageUrl)
        {
            // Get HTML from website
            string htmlContent = string.Empty;
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

            htmlDoc.DocumentNode.SelectNodes("//pre")
                .Append(htmlDoc.CreateTextNode("first line\nsecond line"));

            var preNodes = htmlDoc.DocumentNode.SelectNodes("//pre");
            foreach (HtmlNode htmlPreNode in preNodes)
            {
                var replacedText1 = htmlPreNode.InnerText + "_translated";
                var replacedText2 = htmlPreNode.InnerText.Replace('\n', 'X');
                var replacedText3 = htmlPreNode.InnerText.Replace(System.Environment.NewLine, "Y");
                var replacedText4 = htmlPreNode.InnerText.Replace(System.Environment.NewLine, "NEWLINE");

                var TextWithFormatting = "<span style=\"color: #808080;font-family: Courier New;\">"
                    + replacedText4 + "</span>";

                htmlPreNode.ParentNode.ReplaceChild(
                    HtmlTextNode.CreateNode(
                        TextWithFormatting), htmlPreNode);
            }
            //

            HtmlNode htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]");
            string htmlNodeContent = (htmlNode == null) ? "Error, id not found" : htmlNode.InnerHtml;

            //htmlNode.InnerHtml = htmlNodeContent;


            //// Replace <pre> tag with custom font
            //htmlNodeContent = htmlNodeContent.Replace(
            //    "<pre>", "<span style=\"color: #ff0000;font-family: Courier New;\">");
            //htmlNodeContent = htmlNodeContent.Replace("</pre>", "</span>");

            // replace placeholders with actual <br> tags
            htmlNodeContent = htmlNodeContent.Replace("NEWLINE", "<br />");


            // Write html node's content to text file.
            File.WriteAllText(@"HtmlContent.txt", htmlNodeContent);

            Console.WriteLine("Making your document...");

            // Create DOCX file
            WordDocument wordDoc = new WordDocument($"{outputFileName}.docx");
            wordDoc.Process(new HtmlParser(htmlNodeContent));
            wordDoc.Save();

            //WordDocument doc = new WordDocument("blog.docx");
            //doc.BaseURL = "http:\\wakeupandcode.com";
            //doc.Process(new HtmlParser("<a href=\"index.htm\">sample</a>"));
            //doc.Save();
        }
    }
}
