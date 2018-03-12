using System;
using System.Collections.Generic;
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
                var items = document.QuerySelectorAll(".item-title");
                foreach (var item in items)
                {
                    Console.WriteLine(item.TextContent);
                    using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=GpuLookup;Integrated Security = true"))
                    {
                        try
                        {
                            connection.Open();
                            string sql = "INSERT INTO TEST_TABLE(name) VALUES(@name)";
                            SqlCommand cmd = new SqlCommand(sql, connection);
                            cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = item.TextContent; 
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
    }
}
