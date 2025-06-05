// DAL/LoaiBenhAccess.cs
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTO;

namespace DAL
{
    public class LoaiBenhAccess : DatabaseAccess
    {
        public List<LoaiBenh> GetAll()
        {
            List<LoaiBenh> list = new List<LoaiBenh>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM LOAIBENH";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    LoaiBenh item = new LoaiBenh
                    {
                        ID_LoaiBenh = Convert.ToInt32(reader["ID_LoaiBenh"]),
                        TenLoaiBenh = reader["TenLoaiBenh"].ToString(),
                        TrieuChung = reader["TrieuChung"].ToString(),
                        HuongDieuTri = reader["HuongDieuTri"].ToString()
                    };
                    list.Add(item);
                }
                reader.Close();
            }
            return list;
        }

        public bool Insert(LoaiBenh b)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO LOAIBENH (TenLoaiBenh, TrieuChung, HuongDieuTri) VALUES (@Ten, @TrieuChung, @HuongDieuTri)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Ten", b.TenLoaiBenh);
                cmd.Parameters.AddWithValue("@TrieuChung", b.TrieuChung);
                cmd.Parameters.AddWithValue("@HuongDieuTri", b.HuongDieuTri);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Update(LoaiBenh b)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE LOAIBENH SET TenLoaiBenh = @Ten, TrieuChung = @TrieuChung, HuongDieuTri = @HuongDieuTri WHERE ID_LoaiBenh = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Ten", b.TenLoaiBenh);
                cmd.Parameters.AddWithValue("@TrieuChung", b.TrieuChung);
                cmd.Parameters.AddWithValue("@HuongDieuTri", b.HuongDieuTri);
                cmd.Parameters.AddWithValue("@ID", b.ID_LoaiBenh);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM LOAIBENH WHERE ID_LoaiBenh = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool IsExists(string tenLoaiBenh)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM LOAIBENH WHERE TenLoaiBenh = @Ten";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Ten", tenLoaiBenh);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

    }
}
