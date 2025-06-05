using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTO;

namespace DAL
{
    public class CachDungAccess : DatabaseAccess
    {
        public List<CachDung> GetAll()
        {
            var list = new List<CachDung>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM CACHDUNG";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new CachDung
                    {
                        ID_CachDung = Convert.ToInt32(reader["ID_CachDung"]),
                        MoTaCachDung = reader["MoTaCachDung"].ToString()
                    });
                }
                reader.Close();
            }

            return list;
        }

        public bool Insert(CachDung cd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO CACHDUNG (MoTaCachDung) VALUES (@MoTa)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MoTa", cd.MoTaCachDung);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(CachDung cd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE CACHDUNG SET MoTaCachDung = @MoTa WHERE ID_CachDung = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MoTa", cd.MoTaCachDung);
                cmd.Parameters.AddWithValue("@ID", cd.ID_CachDung);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM CACHDUNG WHERE ID_CachDung = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool IsExists(string moTa)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM CACHDUNG WHERE MoTaCachDung = @MoTa";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MoTa", moTa);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

    }
}
