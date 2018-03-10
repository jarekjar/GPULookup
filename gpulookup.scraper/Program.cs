using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;

namespace GpuLookup.Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new HtmlParser();
            var page1 = "https://www.newegg.com/Desktop-Graphics-Cards/SubCategory/ID-48/";
            WebClient webClient = new WebClient();
            var result = webClient.DownloadString(page1);
            var document = parser.Parse(result);
            Console.WriteLine("Serializing the (original) document:");
            Console.WriteLine(document.DocumentElement.OuterHtml);
            Console.ReadLine();
        }
    }
}
