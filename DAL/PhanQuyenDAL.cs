using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
namespace DAL
{
    public class PhanQuyenDAL: DatabaseAccess
    {

        public List<int> LayDanhSachIdChucNangTheoNhom(int idNhom)
        {
            List<int> ids = new List<int>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_ChucNang FROM PHANQUYEN WHERE ID_Nhom = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idNhom);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ids.Add(reader.GetInt32(0));
                }
            }

            return ids;
        }
        public int? LayNhomTheoEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 ID_NHOM FROM NHANVIEN WHERE Email = @email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", email);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        return reader.GetInt32(0);
                    }
                }
            }

            return null; // hoặc -1 nếu bạn không muốn dùng nullable
        }


        public void LuuPhanQuyen(int idNhom, List<int> dsIdChucNang)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // 1. Xóa quyền cũ
                    SqlCommand deleteCmd = new SqlCommand(
                        "DELETE FROM PHANQUYEN WHERE ID_Nhom = @id",
                        conn,
                        tran);
                    deleteCmd.Parameters.AddWithValue("@id", idNhom);
                    deleteCmd.ExecuteNonQuery();

                    // 2. Thêm quyền mới
                    foreach (int idChucNang in dsIdChucNang)
                    {
                        SqlCommand insertCmd = new SqlCommand(
                            "INSERT INTO PHANQUYEN (ID_Nhom, ID_ChucNang) VALUES (@idNhom, @idChucNang)",
                            conn,
                            tran);
                        insertCmd.Parameters.AddWithValue("@idNhom", idNhom);
                        insertCmd.Parameters.AddWithValue("@idChucNang", idChucNang);
                        insertCmd.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw new Exception("Lỗi khi lưu phân quyền", ex);
                }
            }
        }
        public int? LayIDNhanVienTheoEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 ID_NhanVien FROM NHANVIEN WHERE Email = @email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", email);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        return reader.GetInt32(0);
                    }
                }
            }

            return null; // hoặc -1 nếu bạn muốn không nullable
        }
        public int ThemNhomMoiVaPhanQuyen(string tenNhom, List<int> dsChucNang)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    SqlCommand insertGroup = new SqlCommand(
                        "INSERT INTO NHOMNGUOIDUNG (TenNhom) VALUES (@tenNhom); SELECT SCOPE_IDENTITY();",
                        conn, tran);
                    insertGroup.Parameters.AddWithValue("@tenNhom", tenNhom);

                    int idNhomMoi = Convert.ToInt32(insertGroup.ExecuteScalar());

                    foreach (int idChucNang in dsChucNang)
                    {
                        SqlCommand insertQuyen = new SqlCommand(
                            "INSERT INTO PHANQUYEN (ID_Nhom, ID_ChucNang) VALUES (@idNhom, @idChucNang)",
                            conn, tran);
                        insertQuyen.Parameters.AddWithValue("@idNhom", idNhomMoi);
                        insertQuyen.Parameters.AddWithValue("@idChucNang", idChucNang);
                        insertQuyen.ExecuteNonQuery();
                    }

                    tran.Commit();
                    return idNhomMoi;
                }
                catch
                {
                    tran.Rollback();
                    return -1;
                }
            }
        }
        //public void GhiLog(string email, string moTa)
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        {
        //            string query = "INSERT INTO LOG_TRUYCAP (Email, MoTa) VALUES (@email, @moTa)";
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@email", email);
        //            cmd.Parameters.AddWithValue("@moTa", moTa);
        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}
        //public bool KhoaTaiKhoan(string email)
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        string query = "UPDATE NHANVIEN SET TrangThai = N'Bị khóa' WHERE Email = @Email";
        //        SqlCommand cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@Email", email);
        //        conn.Open();
        //        return cmd.ExecuteNonQuery() > 0;
        //    }
        //}
        //public bool MoKhoaTaiKhoan(string email)
        //{
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        string query = "UPDATE NHANVIEN SET TrangThai = N'Đang làm việc' WHERE Email = @Email";
        //        SqlCommand cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@Email", email);
        //        conn.Open();
        //        return cmd.ExecuteNonQuery() > 0;
        //    }
        //}

    }



}

