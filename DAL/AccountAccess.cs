using System.Data.SqlClient;
using System.Data;
using DTO;
using System.Security.Principal;
using System;
using System.Windows;
using System.Collections.Generic;
namespace DAL
{
    public class AccountAccess : DatabaseAccess
    {
        public DataTable GetUserWithRole(Account acc)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("proc_login", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Thêm tham số cho stored procedure
                cmd.Parameters.AddWithValue("@user", acc.Email);  // Tên tài khoản
                cmd.Parameters.AddWithValue("@pass", acc.MatKhau);    // Mật khẩu

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                return dataTable;
            }
        }

        public string CheckLogin(Account taikhoan, out string userRole)
        {
            userRole = string.Empty; // Khởi tạo giá trị quyền mặc định

            if (string.IsNullOrEmpty(taikhoan.Email))
            {
                return "request_taikhoan"; // Trả về lỗi nếu tài khoản trống
            }
            if (string.IsNullOrEmpty(taikhoan.MatKhau))
            {
                return "request_password"; // Trả về lỗi nếu mật khẩu trống
            }

            // Lấy dữ liệu từ cơ sở dữ liệu và kiểm tra
            DataTable result = GetUserWithRole(taikhoan);
            if (result.Rows.Count > 0)
            {
                // Lấy quyền của người dùng từ kết quả trả về
                userRole = result.Rows[0]["sTenQuyen"].ToString();
                return "success"; // Đăng nhập thành công
            }

            return "invalid_login"; // Nếu không tìm thấy người dùng
        }

       
        
    }
}
