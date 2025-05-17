

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DatabaseAccess
    {
        protected readonly string connectionString = @"Data Source=LAPTOP-5EKP9JC6\SQLEXPRESS01;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        protected SqlConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
        public DataTable GetData(string query)
        {
            DataTable data = new DataTable();
            using (SqlConnection conn = CreateConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(data);
            }
            return data;
        }

    }
}

