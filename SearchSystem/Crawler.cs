using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace SearchSystem
{
    class Crawler
    {
        public List<Document> Index()
        {
            string url = "http://www.rba.ru/content/news/articles/index.php";
            var task = CallUrl2(url);
            task.Wait();
            var response = task.Result;
            var linkList = ParseHtml(response);
            return linkList;
        }

        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

        private static  Task<string> CallUrl2(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.Accept.Clear();
            var webRequest = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            var response = client.GetStringAsync(fullUrl);
            return response;
        }

        private List<Document> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var programmerLinks = htmlDoc.DocumentNode.Descendants("li")
                    .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection")).ToList();

            List<Document> DocsCollection = new List<Document>();

            foreach (var link in programmerLinks)
            {
                if (link.FirstChild.Attributes.Count > 0)
                {
                    HtmlDocument urlContent = new HtmlDocument();
                    var url = "http://www.rba.ru" + link.FirstChild.Attributes[0].Value;

                    var task = CallUrl2(url);                    
                    try
                    {
                        task.Wait();
                        var ans = task.Result;
                        Document doc = new Document();

                        urlContent.LoadHtml(ans);
                        var title = urlContent
                            .DocumentNode.Descendants("h1")
                            .Where(node => node.GetAttributeValue("class", "").Contains("contentheading"))
                            .ToList()[0]
                            .InnerHtml;

                        var content = urlContent
                            .DocumentNode
                            .Descendants("div")
                            .Where(node => node.GetAttributeValue("class", "").Contains("full-article"))
                            .ToList()[0]
                            .InnerHtml;

                        var indexOfTitle = content.IndexOf("</h1>") + 5;
                        doc.title = Parser.removeTagsAndSymbols(content.Substring(0, indexOfTitle));
                        var parsedContent = Parser.removeTagsAndSymbols(content).Trim();
                        doc.date = parsedContent.Substring(parsedContent.Length - 10, 10);
                        doc.text = parsedContent.Remove(parsedContent.Length - 10, 10);
                        doc.time = "";
                        doc.link = url;

                        DocsCollection.Add(doc);
                    }
                    catch
                    {
                        continue;
                    }                                       
                }                    
            }

            return DocsCollection;
        }
    }   
}


