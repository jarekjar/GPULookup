using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
                    Console.WriteLine("You got captcha detected my dude."); 
                } 
                GPU gpu = null;
                string connectionString = ConfigurationManager.ConnectionStrings["conString"].ConnectionString;
                foreach (var item in items)
                {
                    gpu = new GPU();
                    gpu.Card = item.QuerySelector(".item-title").TextContent;
                    if(item.QuerySelector(".price-current > strong") != null)
                    gpu.Price = Double.Parse(item.QuerySelector(".price-current > strong").TextContent) + Double.Parse(item.QuerySelector(".price-current > sup").TextContent);
                    gpu.Source = "Newegg";
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
                page++;
            }
            Console.ReadLine();
        }
        class GPU
        {
            public double Price { get; set; }
            public string Card { get; set; }
            public string Source { get; set; }
        }
    }
}
