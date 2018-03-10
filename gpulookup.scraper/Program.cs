using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;

namespace GpuLookup.Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var gpus = new List<GPU>();
            GPU gpu = null;
            var parser = new HtmlParser();
            var page1 = "https://www.newegg.com/Desktop-Graphics-Cards/SubCategory/ID-48/";
            WebClient webClient = new WebClient();
            var result = webClient.DownloadString(page1);
            var document = parser.Parse(result);
            Console.WriteLine("Serializing the (original) document:");
            var pricesListItemsLinq = document.QuerySelectorAll(".item-brand");
            foreach(var item in pricesListItemsLinq)
            {
                gpu = new GPU();
                string img = item.InnerHtml;
                gpu.ImgTag = Regex.Replace(img, @"\s+", " ");
                gpus.Add(gpu);
            }
            Console.WriteLine(gpus);
            Console.ReadLine();
        }
    }
    class GPU
    {
        public string ImgTag { get; set; }
    }
}
