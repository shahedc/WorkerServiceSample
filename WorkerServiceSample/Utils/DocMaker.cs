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
        public static void MakeDoc(string pageUrl, bool showHtml = false)
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

            if (showHtml)
                Console.WriteLine(htmlContent);

            // Get content
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            htmlDoc.DocumentNode.SelectNodes("//pre")
                .Append(htmlDoc.CreateTextNode("first line\nsecond line"));

            var preNodes = htmlDoc.DocumentNode.SelectNodes("//pre");
            foreach (HtmlNode htmlPreNode in preNodes)
            {
                // replace doc's newline with "NEWLINE" placeholder as HTML tag insertion doesn't seem to work (?)
                var replacedText = htmlPreNode.InnerText.Replace(System.Environment.NewLine, "NEWLINE");

                var TextWithFormatting = "<span style=\"color: #808080;font-family: Courier New;\">"
                    + replacedText + "</span>";

                htmlPreNode.ParentNode.ReplaceChild(
                    HtmlTextNode.CreateNode(
                        TextWithFormatting), htmlPreNode);
            }
            //

            HtmlNode htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]");
            string htmlNodeContent = (htmlNode == null) ? "Error, id not found" : htmlNode.InnerHtml;

            // replace placeholders with actual <br> tags
            htmlNodeContent = htmlNodeContent.Replace("NEWLINE", "<br />");

            // Write html node's content to text file.
            File.WriteAllText(@"HtmlContent.txt", htmlNodeContent);

            Console.WriteLine("Making your document...");

            // Create DOCX file
            WordDocument wordDoc = new WordDocument($"{outputFileName}.docx");
            wordDoc.Process(new HtmlParser(htmlNodeContent));
            wordDoc.Save();
        }
    }
}
