using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class LoginLogDAL : DatabaseAccess
    {
        public void InsertLog(string email, string trangThai, int soLanThatBai, string hanhDong)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
INSERT INTO LOGINLOG (Email, TrangThai, SoLanThatBai, ThoiGian, HanhDong) 
VALUES (@email, @trangThai, @failCount, GETDATE(), @hanhDong)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@trangThai", trangThai);
                cmd.Parameters.AddWithValue("@failCount", soLanThatBai);
                cmd.Parameters.AddWithValue("@hanhDong", hanhDong);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }



        public int CountFailedAttempts(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT TOP 1 SoLanThatBai 
                             FROM LOGINLOG 
                             WHERE Email = @email 
                             ORDER BY ID_Log DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", email);
                conn.Open();

                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
        // Lấy toàn bộ log từ bảng LOGINLOG
        public List<LoginLogDTO> GetAllLogs()
        {
            List<LoginLogDTO> list = new List<LoginLogDTO>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Email, ThoiGian, TrangThai, SoLanThatBai, HanhDong FROM LOGINLOG ORDER BY ThoiGian DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LoginLogDTO
                        {
                            Email = reader["Email"].ToString(),
                            ThoiGian = Convert.ToDateTime(reader["ThoiGian"]),
                            TrangThai = reader["TrangThai"].ToString(),
                            SoLanThatBai = Convert.ToInt32(reader["SoLanThatBai"]),
                            HanhDong = reader["HanhDong"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // Cập nhật thất bại: tăng số lần, kiểm tra và khóa nếu vượt ngưỡng
        public void InsertOrUpdateFailedAttempt(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string checkExist = "SELECT COUNT(*) FROM LOGINLOG WHERE Email = @Email";
                SqlCommand checkCmd = new SqlCommand(checkExist, conn);
                checkCmd.Parameters.AddWithValue("@Email", email);

                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    string update = @"
                        UPDATE LOGINLOG
                        SET 
                            ThoiGian = GETDATE(),
                            SoLanThatBai = SoLanThatBai + 1,
                            TrangThai = CASE WHEN SoLanThatBai + 1 >= 5 THEN N'Bị khóa' ELSE TrangThai END
                        WHERE Email = @Email";
                    SqlCommand updateCmd = new SqlCommand(update, conn);
                    updateCmd.Parameters.AddWithValue("@Email", email);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    string insert = @"
                        INSERT INTO LOGINLOG (Email, SoLanThatBai, TrangThai)
                        VALUES (@Email, 1, N'Đang làm việc')";
                    SqlCommand insertCmd = new SqlCommand(insert, conn);
                    insertCmd.Parameters.AddWithValue("@Email", email);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        // Reset trạng thái khi mở khóa
        public void MoKhoaTaiKhoan(string email, string trangThai)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                UPDATE LOGINLOG 
                SET TrangThai = @TrangThai, SoLanThatBai = 0 
                WHERE Email = @Email";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.ExecuteNonQuery();
            }
        }

        // Lấy trạng thái hiện tại của email
        public string GetTrangThaiByEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TrangThai FROM LOGINLOG WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                object result = cmd.ExecuteScalar();
                return result?.ToString() ?? "Không có dữ liệu";
            }
        }
        public bool XacThucTaiKhoan(string email, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM NHANVIEN WHERE Email = @Email AND MatKhau = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                conn.Open();

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }


    }

}
