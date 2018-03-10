using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IronWebScraper;

namespace GpuLookup.Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var page1 = "https://www.newegg.com/Desktop-Graphics-Cards/SubCategory/ID-48/";
            WebClient webClient = new WebClient();
            var result = webClient.DownloadString(page1);
            string replaceWith = "";
            string match = "<a\\s+(?:[^>]*?\\s+)?href=\"([^ \"]*)\"";
            result = result.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
            var image_link_match = Regex.Match(result, match).Value;
            Console.WriteLine(image_link_match);
            Console.ReadLine();
        }
    }
}
