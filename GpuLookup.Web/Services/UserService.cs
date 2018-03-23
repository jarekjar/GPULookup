using GpuLookup.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static GpuLookup.Controllers.GPUController;
using BCrypt.Net; 

namespace GpuLookup.Services
{
    public class UserService
    {
        public void InsertUser(User user)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("users_insert", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = user.Username;
                cmd.Parameters.Add("@password_hash", SqlDbType.NVarChar).Value = passwordHash;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool Login(User user)
        {
            string passwordHash = null;
            SqlDataReader rdr = null;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("users_login", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@username", user.Username);
                conn.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    passwordHash = (string)rdr[0];
                }
            }
            if (passwordHash == null)
                throw new LoginException();
            if (BCrypt.Net.BCrypt.Verify(user.Password, passwordHash))
                return true;
            else
                return false;
        }
    }
}