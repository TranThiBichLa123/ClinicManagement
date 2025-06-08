using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    using System;
    using System.Data.SqlClient;

    namespace DAL
    {
        public class NhanVienDAL : DatabaseAccess
        {
            public string GetTrangThaiNhanVien(string email)
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT TrangThai FROM NHANVIEN WHERE Email = @Email", conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Không rõ";
                }
            }

            public void UpdateTrangThai(string email, string newTrangThai)
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("UPDATE NHANVIEN SET TrangThai = @TrangThai WHERE Email = @Email", conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@TrangThai", newTrangThai);
                    cmd.ExecuteNonQuery();
                }
            }

            public bool CheckEmailExists(string email)
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT COUNT(*) FROM NHANVIEN WHERE Email = @Email", conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }

            public int? GetIDNhanVienByEmail(string email)
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT ID_NhanVien FROM NHANVIEN WHERE Email = @Email", conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : (int?)null;
                }
            }

            public void CapNhatTrangThai(string email, string trangThai)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE NHANVIEN SET TrangThai = @TrangThai WHERE Email = @Email";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.ExecuteNonQuery();
                }
            }

        }
    }

}
