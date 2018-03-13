using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;

namespace GpuLookup.Scraper
{
    class Program
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString;
        static void Main(string[] args)
        {
            //NeweggScrape();
            //AmazonScrape();
            //MicrocenterScrape();
            BestBuyScrape();
            Console.WriteLine("Completed.");
            Console.ReadLine();
        }
        static void NeweggScrape()
        {
            var parser = new HtmlParser();
            var page1 = "https://www.newegg.com/Desktop-Graphics-Cards/SubCategory/ID-48/";
            WebClient webClient = new WebClient();
            var page = 1;
            while (page < 60)
            {
                string result = null;
                if (page == 1)
                {
                    result = webClient.DownloadString(page1);
                }
                else
                {
                    result = webClient.DownloadString(page1 + "Page-" + page);
                }
                var document = parser.Parse(result);
                var items = document.QuerySelectorAll(".item-info");
                if (items.Length == 0)
                {
                    Console.WriteLine("reCaptcha has detected that you are, in fact, actually a robot.");
                    Console.ReadLine();
                }
                GPU gpu = null;
                foreach (var item in items)
                {
                    gpu = new GPU();
                    gpu.Card = item.QuerySelector(".item-title").TextContent;
                    if (item.QuerySelector(".price-current > strong") != null)
                        gpu.Price = Double.Parse(item.QuerySelector(".price-current > strong").TextContent) + Double.Parse(item.QuerySelector(".price-current > sup").TextContent);
                    gpu.Source = "Newegg";
                    SQLInsert(gpu);
                }
                page++;
            }
        }
        static void AmazonScrape()
        {
            var parser = new HtmlParser();
            var page1 = "https://www.amazon.com/Graphics-Cards-Computer-Add-Ons-Computers/b/ref=dp_bc_4?ie=UTF8&node=284822";
            WebClient webClient = new WebClient();
            var page = 1;
            while (page < 40)
            {
                string result = null;
                result = webClient.DownloadString(page1 + "&page=" + page);
                var document = parser.Parse(result);
                var items = document.QuerySelectorAll(".s-item-container");
                if (items.Length == 0)
                {
                    Console.WriteLine("reCaptcha has detected that you are, in fact, actually a robot.");
                    Console.ReadLine();
                }
                GPU gpu = null;
                foreach (var item in items)
                {
                    gpu = new GPU();
                    if (item.QuerySelector(".s-access-title") != null)
                        gpu.Card = item.QuerySelector(".s-access-title").TextContent;
                    else
                        continue;
                    if (item.QuerySelector(".sx-price-whole") != null)
                        gpu.Price = Double.Parse(item.QuerySelector(".sx-price-whole").TextContent) + Double.Parse(item.QuerySelector(".sx-price-fractional").TextContent);
                    gpu.Source = "Amazon";
                    SQLInsert(gpu);
                }
                page++;
            }
        }
        static void MicrocenterScrape()
        {
            var parser = new HtmlParser();
            var page1 = "http://www.microcenter.com/search/search_results.aspx?N=4294966937&NTK=all&cat=Video-Cards-:-Video-Cards,-TV-Tuners-:-Computer-Parts-:-MicroCenter";
            WebClient webClient = new WebClient();
            var page = 1;
            while (page < 8)
            {
                string result = null;
                result = webClient.DownloadString(page1 + "&page=" + page);
                var document = parser.Parse(result);
                var items = document.QuerySelectorAll(".product_wrapper");
                if (items.Length == 0)
                {
                    Console.WriteLine("reCaptcha has detected that you are, in fact, actually a robot.");
                    Console.ReadLine();
                }
                GPU gpu = null;
                foreach (var item in items)
                {
                    gpu = new GPU();
                    if (item.QuerySelector(".normal > h2 > a") != null)
                        gpu.Card = item.QuerySelector(".normal > h2 > a").GetAttribute("data-name");
                    else
                        continue;
                    gpu.Price = Double.Parse(item.QuerySelector(".normal > h2 > a").GetAttribute("data-price"));
                    gpu.Source = "Microcenter";
                    SQLInsert(gpu);
                }
                page++;
            }
        }
        static void BestBuyScrape()
        {
            var parser = new HtmlParser();
            WebClient webClient = new WebClient();
            var client = new HttpClient();
            var page = 1;
            var timeout = DateTime.Now;
            while (page < 5)
            {
                string result = null;
                string url = String.Format("http://www.bestbuy.com/site/searchpage.jsp?cp={0}&searchType=search&_dyncharset=UTF-8&ks=960&sc=Global&list=y&usc=All%20Categories&type=page&id=pcat17071&iht=n&seeAll=&browsedCategory=abcat0507002&st=categoryid%24abcat0507002&qp=&sp=-bestsellingsort%20skuidsaas", page);
                
                Task.Run(async () => {
                    result = await client.GetStringAsync(url);
                    var document = parser.Parse(result);
                    var items = document.QuerySelectorAll(".list-item");
                    if (items.Length == 0)
                    {
                        Console.WriteLine("reCaptcha has detected that you are, in fact, actually a robot.");
                        Console.ReadLine();
                    }
                    GPU gpu = null;
                    foreach (var item in items)
                    {
                        gpu = new GPU();
                        if (item != null)
                            gpu.Card = item.GetAttribute("data-title");
                        else
                            continue;
                        gpu.Price = Double.Parse(item.GetAttribute("data-price"));
                        gpu.Source = "Best Buy";
                        SQLInsert(gpu);
                    }
                    page++;
                });

                if(DateTime.Now - timeout > TimeSpan.FromSeconds(10))
                {
                    Console.WriteLine("Timed out after 10 seconds.");
                    Console.ReadLine();
                    break;
                }
            }
        }
        static void SQLInsert(GPU gpu)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Inserting " + gpu.Card + " into the database.");
                    string sql = "INSERT INTO GPUS(chip, price, source) VALUES(@chip, @price, @source)";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.Add("@chip", SqlDbType.NVarChar).Value = gpu.Card;
                    cmd.Parameters.Add("@price", SqlDbType.Money).Value = gpu.Price;
                    cmd.Parameters.Add("@source", SqlDbType.NVarChar).Value = gpu.Source;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        class GPU
        {
            public double Price { get; set; }
            public string Card { get; set; }
            public string Source { get; set; }
        }
    }
}
