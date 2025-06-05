using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using DTO;
namespace DAL
{
    public class SettingAccess:DatabaseAccess
    {

        public List<Setting> GetRoleList()
        {
            List<Setting> settingList = new List<Setting>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
SELECT 
    N.ID_NHOM,
    N.TENNHOM
  
FROM NHOMNGUOIDUNG N

";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Setting setting = new Setting
                    {
                        ID_VaiTro = Convert.ToInt32(reader["ID_NHOM"]),
                        TenVaiTro = reader["TENNHOM"].ToString(),
                       

                    };
                    settingList.Add(setting);
                }
                reader.Close();
            }
            return settingList;
        }
     
        public List<Account> GetAccountList()
        {
            List<Account> settingList = new List<Account>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
SELECT 
   N.ID_NHOM,
    NV.ID_NHANVIEN,
    NV.MATKHAU,
    NV.EMAIL,
    NV.TRANGTHAI
FROM NHANVIEN NV
JOIN NHOMNGUOIDUNG N ON NV.ID_NHOM = N.ID_NHOM
";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Account setting = new Account
                    {
                        ID_TaiKhoan = Convert.ToInt32(reader["ID_NHANVIEN"]),
                        TrangThai = reader["TrangThai"].ToString(),
                        Email = reader["Email"].ToString(),
                        MatKhau = reader["MatKhau"].ToString(),
                        ID_VaiTro = Convert.ToInt32(reader["ID_NHOM"]),


                    };
                    settingList.Add(setting);
                }
                reader.Close();
            }
            return settingList;
        }

 
        public List<Staff> GetStaffList()
        {
            List<Staff> settingList = new List<Staff>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
SELECT 
    NH.ID_NHOM,    
n.TrangThai,
n.ID_NhanVien,
n.HoTenNV,
n.NgaySinh,
n.GioiTinh,
n.CCCD,
n.DienThoai,
n.DiaChi,
n.TrangThai,
n.Email,
n.HinhAnh
FROM NhanVien n
JOIN NHOMNGUOIDUNG NH ON n.ID_NHOM = NH.ID_NHOM
";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Staff setting = new Staff
                    {
                        ID_VaiTro = Convert.ToInt32(reader["ID_NHOM"]),
                        ID_NhanVien = Convert.ToInt32(reader["ID_NhanVien"]),

                        Email = reader["Email"].ToString(),
                        HoTenNV = reader["HoTenNV"].ToString(),
                        NgaySinh = reader["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(reader["NgaySinh"]) : (DateTime?)null,
                        HinhAnh = reader["HinhAnh"].ToString(),
                        GioiTinh = reader["GioiTinh"].ToString(),
                        CCCD = reader["CCCD"].ToString(),
                        DienThoai = reader["DienThoai"].ToString(),
                        DiaChi = reader["DiaChi"].ToString(),
                        TrangThai = reader["TrangThai"].ToString(),



                    };
                    settingList.Add(setting);
                }
                reader.Close();
            }
            return settingList;
        }



        /*public bool InsertRole(Setting role)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO VaiTro (ID_VaiTro, TenVaiTro, TrangThai) VALUES (@ID, @Ten, 1)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", role.ID_VaiTro);
                    cmd.Parameters.AddWithValue("@Ten", role.TenVaiTro);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in InsertRole: " + ex.Message);
                return false;
            }
        }

        public bool UpdateRole(Setting role)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE VaiTro SET TenVaiTro = @Ten WHERE ID_VaiTro = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", role.ID_VaiTro);
                    cmd.Parameters.AddWithValue("@Ten", role.TenVaiTro);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateRole: " + ex.Message);
                return false;
            }
        }

        public bool DeleteRole(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE VaiTro SET TrangThai = 0 WHERE ID_VaiTro = @ID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in DeleteRole: " + ex.Message);
                return false;
            }
        }*/




    }
}
