using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTO;

namespace DAL
{
    public class DonViTinhAccess : DatabaseAccess
    {
        public List<DonViTinh> GetAll()
        {
            List<DonViTinh> list = new List<DonViTinh>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM DVT";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new DonViTinh
                    {
                        ID_DVT = Convert.ToInt32(reader["ID_DVT"]),
                        TenDVT = reader["TenDVT"].ToString()
                    });
                }

                reader.Close();
            }

            return list;
        }

        public bool IsExists(string tenDVT)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM DVT WHERE TenDVT = @Ten";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Ten", tenDVT);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }


        public bool Insert(DonViTinh dvt)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO DVT (TenDVT) VALUES (@TenDVT)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenDVT", dvt.TenDVT);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(DonViTinh dvt)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE DVT SET TenDVT = @TenDVT WHERE ID_DVT = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenDVT", dvt.TenDVT);
                cmd.Parameters.AddWithValue("@ID", dvt.ID_DVT);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM DVT WHERE ID_DVT = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
