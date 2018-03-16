using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace GpuLookup.Scraper
{
    class Program
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString;
        static string[] proxyList = new string[] {
            "207.246.99.38:8080",
            "144.202.55.61:8080",
            "207.246.106.194:8080",
            "35.199.63.39:8080",
            "207.246.110.64:8080",
            "207.246.99.75:8080",
            "144.202.112.88:8080",
            "138.68.236.249:3128",
            "191.101.156.89:80"
        };
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to scrape Newegg, Amazon, Microcenter, and Best Buy.");
            Console.ReadLine();
            TruncateTable();
            Task.WaitAll(
                //NeweggScrape(),
                AmazonScrape(),
                MicrocenterScrape(),
                BestBuyScrape()
            );
            Console.WriteLine("Filter the SQL Table");
            FilterDuplicates();
            Console.WriteLine("Completed.");
            Console.ReadLine();
        }
        static async Task NeweggScrape()
        {
            var parser = new HtmlParser();
            var page1 = "https://www.newegg.com/Desktop-Graphics-Cards/SubCategory/ID-48/?recaptcha=pass&PageSize=96";
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--disable-javascript");
            options.AddArgument("--disable-images");
            options.AddArgument("incognito"); 
            options.AddArgument("--disable-bundled-ppapi-flash"); 
            options.AddArgument("--disable-extensions");
            options.AddArgument("--headless");
            var rnd = new Random();
            options.AddArgument("--proxy-server=http://" + proxyList[rnd.Next(1,proxyList.Length - 1)]); 
            IWebDriver chromeDriver = new ChromeDriver(options);
            string result = null;
            var page = 1;
            chromeDriver.Url = page1;
            while (page < 30)
            {
                if (page != 1)
                    chromeDriver.Url = "https://www.newegg.com/Desktop-Graphics-Cards/SubCategory/ID-48/" + "Page-" + page + "?&PageSize=96";
                result = chromeDriver.PageSource;
                var document = parser.Parse(result);
                var items = document.QuerySelectorAll(".item-container");
                //item-info
                if (items.Length < 90)
                {
                    continue;
                }
                GPU gpu = null;
                foreach (var item in items)
                {
                    gpu = new GPU();
                    gpu.Card = item.QuerySelector(".item-title").TextContent;
                    if (item.QuerySelector(".price-current > strong") != null)
                        gpu.Price = Double.Parse(item.QuerySelector(".price-current > strong").TextContent) + Double.Parse(item.QuerySelector(".price-current > sup").TextContent);
                    gpu.Source = "Newegg";
                    gpu.Url = item.QuerySelector(".item-img").Attributes["href"].Value;
                    gpu.ImageUrl = "http:" + item.QuerySelector(".item-img").FirstElementChild.Attributes["src"].Value;
                    SQLInsert(gpu);
                }
                page++;
            }
            chromeDriver.Close();
        }
        static async Task AmazonScrape()
        {
            var parser = new HtmlParser();
            var page1 = "https://www.amazon.com/Graphics-Cards-Computer-Add-Ons-Computers/b/ref=dp_bc_4?ie=UTF8&node=284822";
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            IWebDriver chromeDriver = new ChromeDriver(options);
            var page = 1;
            chromeDriver.Url = page1 + "&page=" + page;
            bool pageLoading = true;
            while (page < 40)
            {
                string result = null;
                if(!pageLoading)
                    chromeDriver.Url = page1 + "&page=" + page;
                result = chromeDriver.PageSource;
                var document = parser.Parse(result);
                var items = document.QuerySelectorAll(".s-item-container");
                if (items.Length == 0)
                {
                    pageLoading = true;
                    continue;
                }
                pageLoading = false;
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
                    gpu.ImageUrl = item.QuerySelector(".s-access-image").GetAttribute("src");
                    gpu.Url = item.QuerySelector(".a-link-normal").GetAttribute("href");
                    SQLInsert(gpu);
                }
                page++;
            }
            chromeDriver.Close();
        }
        static async Task MicrocenterScrape()
        {
            var parser = new HtmlParser();
            var page1 = "http://www.microcenter.com/search/search_results.aspx?N=4294966937&NTK=all&cat=Video-Cards-:-Video-Cards,-TV-Tuners-:-Computer-Parts-:-MicroCenter";
            WebClient webClient = new WebClient();
            var page = 1;
            while (page < 8)
            {
                string result = null;
                result = await webClient.DownloadStringTaskAsync(page1 + "&page=" + page);
                var document = parser.Parse(result);
                var items = document.QuerySelectorAll(".product_wrapper");
                if (items.Length == 0)
                {
                    Console.WriteLine("reCaptcha has detected that you are, in fact, actually a robot.");
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
                    gpu.ImageUrl = item.QuerySelector(".SearchResultProductImage").GetAttribute("src");
                    gpu.Url = "http://www.microcenter.com" + item.QuerySelector(".normal > h2 > a").GetAttribute("href");
                    SQLInsert(gpu);
                }
                page++;
            }
        }
        static async Task BestBuyScrape()
        {
            var parser = new HtmlParser();
            WebClient webClient = new WebClient();
            var page = 1;
            while (page < 5)
            {
                var result = await Task.Run(() => ComplicatedBestBuyScrape(page));
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
                    gpu.Url = "https://www.bestbuy.com" + item.GetAttribute("data-url");
                    gpu.ImageUrl = "https://www.bestbuy.com" + item.GetAttribute("data-img-path");
                    SQLInsert(gpu);
                }
                page++;
            }
        }
        static string ComplicatedBestBuyScrape(int i)
        {
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect("www.bestbuy.com", 443);
                using (var ssl = new SslStream(tcpClient.GetStream()))
                {
                    ssl.AuthenticateAsClient("www.bestbuy.com");
                    var request = "GET /site/computer-cards-components/video-graphics-cards/abcat0507002.c?id=abcat0507002&cp=" + i + @"&nrp=24 HTTP/1.1
Host: www.bestbuy.com
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
Referer: https://www.bestbuy.com/site/computer-cards-components/video-graphics-cards/abcat0507002.c?id=abcat0507002
Accept-Encoding: gzip, deflate, br
Accept-Language: en-US,en;q=0.9

";
                    ssl.Write(Encoding.UTF8.GetBytes(request));
                    ssl.Flush();
                    Console.WriteLine("sent");
                    // skip headers (look for 2 \r\n consecutively, per the HTTP spec)
                    byte c1 = (byte)ssl.ReadByte();
                    byte c2 = (byte)ssl.ReadByte();
                    byte c3 = (byte)ssl.ReadByte();
                    byte c4 = (byte)ssl.ReadByte();
                    while (c1 != '\r' || c2 != '\n' || c3 != '\r' || c4 != '\n')
                    {
                        c1 = c2;
                        c2 = c3;
                        c3 = c4;
                        c4 = (byte)ssl.ReadByte();
                    }
                    var chunks = new List<byte>();
                    while (true)
                    {
                        // process chunked encoding
                        var bytes = new List<byte>();
                        while (bytes.Count < 2 || bytes[bytes.Count - 2] != '\r' || bytes[bytes.Count - 1] != '\n')
                        {
                            bytes.Add((byte)ssl.ReadByte());
                        }
                        bytes.RemoveRange(bytes.Count - 2, 2);
                        var chunkSize = int.Parse(Encoding.ASCII.GetString(bytes.ToArray()), System.Globalization.NumberStyles.HexNumber);
                        if (chunkSize == 0)
                            break;
                        var buffer = new byte[chunkSize];
                        var leftToRead = buffer.Length;
                        while (leftToRead > 0)
                        {
                            var bytesRead = ssl.Read(buffer, buffer.Length - leftToRead, leftToRead);
                            leftToRead -= bytesRead;
                        }
                        chunks.AddRange(buffer);
                        byte b = (byte)ssl.ReadByte(); // \r
                        Debug.Assert(b == '\r');
                        b = (byte)ssl.ReadByte(); // \n
                        Debug.Assert(b == '\n');
                    }
                    using (var ms = new MemoryStream(chunks.ToArray(), writable: false))
                    using (var gzip = new GZipStream(ms, CompressionMode.Decompress))
                    using (var reader = new StreamReader(gzip))
                    {
                        return reader.ReadToEnd();
                    }
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
                    string sql = "INSERT INTO GPUS(chip, price, source, url, image_url) VALUES(@chip, @price, @source, @url, @image_url)";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.Add("@chip", SqlDbType.NVarChar).Value = gpu.Card;
                    cmd.Parameters.Add("@price", SqlDbType.Money).Value = gpu.Price;
                    cmd.Parameters.Add("@source", SqlDbType.NVarChar).Value = gpu.Source;
                    cmd.Parameters.Add("@url", SqlDbType.NVarChar).Value = gpu.Url;
                    cmd.Parameters.Add("@image_url", SqlDbType.NVarChar).Value = gpu.ImageUrl;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        static void FilterDuplicates()
        {
            using (var conn = new SqlConnection(connectionString))
            using (var command = new SqlCommand("GPUS_deleteDuplicates", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                conn.Open();
                command.ExecuteNonQuery();
            }
        }
        static void TruncateTable()
        {
            using (var conn = new SqlConnection(connectionString))
            using (var command = new SqlCommand("TRUNCATE TABLE GPUS", conn)
            {
                CommandType = CommandType.Text
            })
            {
                conn.Open();
                command.ExecuteNonQuery();
            }
        }
        public class GPU
        {
            public double Price { get; set; }
            public string Card { get; set; }
            public string Source { get; set; }
            public string Url { get; set; }
            public string ImageUrl { get; set; }
        }
    }
}
