using GpuLookup.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static GpuLookup.Controllers.GPUController;

namespace GpuLookup.Services
{
    public class GpuService
    {
        public List<GPU> GetAll()
        {
            List<GPU> gpus = null;
            GPU gpu = null;
            SqlDataReader rdr = null;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select chip, price, source, url, image_url from GPUS", conn);
                conn.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (gpus == null)
                        gpus = new List<GPU>();
                    gpu = new GPU();
                    gpu.Card = rdr.GetString(0);
                    gpu.Price = rdr.GetDecimal(1);
                    gpu.Source = rdr.GetString(2);
                    gpu.Url = rdr.GetString(3);
                    gpu.ImageUrl = rdr.GetString(4);
                    gpus.Add(gpu);
                }
            }
            return gpus;
        }

        public List<GPU> GetResults(string query)
        {
            List<GPU> gpus = null;
            GPU gpu = null;
            SqlDataReader rdr = null;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select chip, price, source, url, image_url FROM GPUS WHERE GPUS.chip LIKE '%' + @string + '%'; ", conn);
                cmd.Parameters.Add("@string", SqlDbType.NVarChar).Value = query;
                conn.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (gpus == null)
                        gpus = new List<GPU>();
                    gpu = new GPU();
                    gpu.Card = rdr.GetString(0);
                    gpu.Price = rdr.GetDecimal(1);
                    gpu.Source = rdr.GetString(2);
                    gpu.Url = rdr.GetString(3);
                    gpu.ImageUrl = rdr.GetString(4);
                    gpus.Add(gpu);
                }
            }
            return gpus;
        }

        public List<GPU> GetNext(Paginate Data)
        {
            List<GPU> gpus = null;
            GPU gpu = null;
            SqlDataReader rdr = null;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("GPUS_getNext20", conn){ CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add("@pageNumber", SqlDbType.Int).Value = Data.PageNum;
                cmd.Parameters.Add("@sort", SqlDbType.NVarChar).Value = Data.SortBy;
                cmd.Parameters.Add("@asc", SqlDbType.Bit).Value = Data.Ascending;
                cmd.Parameters.Add("@query", SqlDbType.NVarChar).Value = Data.Query;
                conn.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (gpus == null)
                        gpus = new List<GPU>();
                    gpu = new GPU();
                    gpu.Card = rdr.GetString(0);
                    gpu.Price = rdr.GetDecimal(1);
                    gpu.Source = rdr.GetString(2);
                    gpu.Url = rdr.GetString(3);
                    gpu.ImageUrl = rdr.GetString(4);
                    gpu.RowCount = rdr.GetInt32(5);
                    gpus.Add(gpu);
                }
            }
            return gpus;
        }
    }
}