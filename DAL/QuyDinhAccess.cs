using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTO;

namespace DAL
{
    public class QuiDinhAccess : DatabaseAccess
    {
        public List<QuiDinh> GetAllQuiDinh()
        {
            List<QuiDinh> list = new List<QuiDinh>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM QUI_DINH";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    QuiDinh item = new QuiDinh
                    {
                        TenQuiDinh = reader["TenQuiDinh"].ToString(),
                        GiaTri = Convert.ToDouble(reader["GiaTri"])
                    };
                    list.Add(item);
                }
                reader.Close();
            }
            return list;
        }

        public bool InsertQD(QuiDinh qd)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO QUI_DINH (TenQuiDinh, GiaTri) VALUES (@TenQuiDinh, @GiaTri)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TenQuiDinh", qd.TenQuiDinh);
                    cmd.Parameters.AddWithValue("@GiaTri", qd.GiaTri);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("InsertQD Error: " + ex.Message);
                return false;
            }
        }

        public bool UpdateQD(QuiDinh qd)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE QUI_DINH SET GiaTri = @GiaTri WHERE TenQuiDinh = @TenQuiDinh";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@GiaTri", qd.GiaTri);
                    cmd.Parameters.AddWithValue("@TenQuiDinh", qd.TenQuiDinh);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateQD Error: " + ex.Message);
                return false;
            }
        }

        public bool DeleteQD(string tenQuiDinh)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM QUI_DINH WHERE TenQuiDinh = @TenQuiDinh";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TenQuiDinh", tenQuiDinh);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteQD Error: " + ex.Message);
                return false;
            }
        }

        public bool IsExists(string ten)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM QUI_DINH WHERE TenQuiDinh = @Ten";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Ten", ten);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

    }
}
