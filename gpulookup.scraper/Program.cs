using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GpuLookup.Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var page1 = "https://www.newegg.com/Desktop-Graphics-Cards/SubCategory/ID-48/";
            WebClient webClient = new WebClient();
            var result = webClient.DownloadString(page1);
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
