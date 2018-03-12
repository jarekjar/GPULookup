using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpuLookup.Parser
{
    class Program
    {
        static void Main()
        {
            string str = "Data Source=localhost;Initial Catalog=GpuLookup;Integrated Security=True";
            Console.WriteLine("Press enter to begin.");
            Console.ReadLine();
            ParseTitles(str);
        }

        private static void ParseTitles(string connectionString)
        {
            var gpu = new GPU();
            List<GPU> gpus = null;
            string queryString = "SELECT * FROM TEST_TABLE";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if(gpus == null)
                    {
                        gpus = new List<GPU>();
                    }
                    string[] titleWords = reader.GetString(0).Split();
                    gpu.Manufacturer = titleWords[0];
                    titleWords[0] = "";
                    gpu.ChipName = String.Join(" ", titleWords);
                    gpus.Add(gpu);
                    gpu = new GPU();
                }
                reader.Close();
            }
            Console.WriteLine("Finished reading data, press enter to continue..");
            Console.ReadLine();
            InsertGpus(gpus, connectionString);
            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static void InsertGpus(List<GPU> gpus, string conString)
        {
            foreach (var gpu in gpus)
            {
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    try
                    {
                        connection.Open();
                        string sql = "INSERT INTO TEST_TABLE_SPECIFICS (manufacturer, chip_name) VALUES(@manufacturer, @chip_name)";
                        SqlCommand cmd = new SqlCommand(sql, connection);
                        cmd.Parameters.Add("@manufacturer", SqlDbType.NVarChar).Value = gpu.Manufacturer;
                        cmd.Parameters.Add("@chip_name", SqlDbType.NVarChar).Value = gpu.ChipName;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
    class GPU
    {
        public string Manufacturer { get; set; }
        public string ChipName { get; set; }
    }
}
