using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DTO;

namespace DAL
{
    public class RoleManagementAccess : DatabaseAccess
    {
        // Lấy danh sách tất cả nhóm người dùng
        public List<RoleManagement> GetAllRoles()
        {
            var roles = new List<RoleManagement>();
            string query = "SELECT ID_Nhom AS ID_VaiTro, TenNhom AS TenVaiTro FROM NHOMNGUOIDUNG";

            DataTable table = GetData(query);
            foreach (DataRow row in table.Rows)
            {
                roles.Add(new RoleManagement
                {
                    ID_VaiTro = row.Field<int>("ID_VaiTro"),
                    TenVaiTro = row.Field<string>("TenVaiTro")
                });
            }

            return roles;
        }

        // Thêm nhóm quyền mới
        public bool InsertRole(string tenVaiTro)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO NHOMNGUOIDUNG (TenNhom) VALUES (@TenNhom)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TenNhom", tenVaiTro);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Cập nhật tên nhóm quyền
        public bool UpdateRole(int idVaiTro, string tenVaiTro)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE NHOMNGUOIDUNG SET TenNhom = @TenNhom WHERE ID_Nhom = @ID_Nhom";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TenNhom", tenVaiTro);
                    cmd.Parameters.AddWithValue("@ID_Nhom", idVaiTro);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Xoá nhóm quyền theo ID
        public bool DeleteRole(int idVaiTro)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM NHOMNGUOIDUNG WHERE ID_Nhom = @ID_Nhom";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID_Nhom", idVaiTro);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }



        public List<AccountViewModel> GetAllAccounts()
        {
            var list = new List<AccountViewModel>();
            string query = @"
        SELECT 
            ID_NhanVien AS ID_TaiKhoan, 
            Email, 
            MatKhau, 
            ID_Nhom AS ID_VaiTro, 
            TrangThai
        FROM NHANVIEN";

            var table = GetData(query);
            foreach (DataRow row in table.Rows)
            {
                list.Add(new AccountViewModel
                {
                    ID_TaiKhoan = row.Field<int>("ID_TaiKhoan"),
                    Email = row.Field<string>("Email"),
                    MatKhau = row.Field<string>("MatKhau"),
                    ID_VaiTro = row.Field<int>("ID_VaiTro"),
                    TrangThai = row.Field<string>("TrangThai")
                });
            }

            return list;
        }


    }
}
