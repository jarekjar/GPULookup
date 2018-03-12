using System;
using System.Collections.Generic;
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
            ParseTitles(str);
        }

        private static void ParseTitles(string connectionString)
        {
            string queryString = "SELECT * FROM TEST_TABLE";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string title = reader.GetString(0);
                }
                reader.Close();
            }
        }
    }
    class GPU
    {
        public string Manufacturer { get; set; }
        public string ChipName { get; set; }
    }
}
