using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DTO;

namespace DAL
{
    public class StaffAccountAccess : DatabaseAccess
    {
        public List<StaffAccount> GetRoleList()
        {
            var result = new List<StaffAccount>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ID_NHOM, TENNHOM FROM NHOMNGUOIDUNG", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new StaffAccount
                        {
                            ID_VaiTro = (int)reader["ID_NHOM"],
                            TenVaiTro = reader["TENNHOM"].ToString()
                        });
                    }
                }
            }
            return result;
        }

        public List<AccountManager> GetAccountList()
        {
            var result = new List<AccountManager>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT NV.ID_NHANVIEN, NV.MATKHAU, NV.EMAIL, NV.TRANGTHAI, N.ID_NHOM 
                                          FROM NHANVIEN NV 
                                          JOIN NHOMNGUOIDUNG N ON NV.ID_NHOM = N.ID_NHOM", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new AccountManager
                        {
                            ID_TaiKhoan = (int)reader["ID_NHANVIEN"],
                            MatKhau = reader["MATKHAU"].ToString(),
                            Email = reader["EMAIL"].ToString(),
                            TrangThai = reader["TRANGTHAI"].ToString(),
                            ID_VaiTro = (int)reader["ID_NHOM"]
                        });
                    }
                }
            }
            return result;
        }

        public List<Staff> GetStaffList()
        {
            var result = new List<Staff>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT n.ID_NhanVien, n.HoTenNV, n.NgaySinh, n.GioiTinh, n.CCCD, n.DienThoai, 
                                                  n.DiaChi, n.TrangThai, n.Email, n.HinhAnh, n.MatKhau, NH.ID_NHOM 
                                           FROM NhanVien n 
                                           JOIN NHOMNGUOIDUNG NH ON n.ID_NHOM = NH.ID_NHOM", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Staff
                        {
                            ID_NhanVien = (int)reader["ID_NhanVien"],
                            HoTenNV = reader["HoTenNV"].ToString(),
                            NgaySinh = reader["NgaySinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgaySinh"]),
                            GioiTinh = reader["GioiTinh"].ToString(),
                            CCCD = reader["CCCD"].ToString(),
                            DienThoai = reader["DienThoai"].ToString(),
                            DiaChi = reader["DiaChi"].ToString(),
                            TrangThai = reader["TrangThai"].ToString(),
                            Email = reader["Email"].ToString(),
                            HinhAnh = reader["HinhAnh"].ToString(),
                            Mat_Khau = reader["MatKhau"].ToString(),
                            ID_VaiTro = (int)reader["ID_NHOM"]
                        });
                    }
                }
            }
            return result;
        }

        public bool InsertStaff(Staff nv)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO NHANVIEN (HoTenNV, NgaySinh, GioiTinh, CCCD, DienThoai, DiaChi, Email, HinhAnh, ID_Nhom, TrangThai, MatKhau)
                                     VALUES (@HoTenNV, @NgaySinh, @GioiTinh, @CCCD, @DienThoai, @DiaChi, @Email, @HinhAnh, @ID_Nhom, @TrangThai, @MatKhau)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@HoTenNV", nv.HoTenNV);
                    cmd.Parameters.AddWithValue("@NgaySinh", nv.NgaySinh ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GioiTinh", nv.GioiTinh);
                    cmd.Parameters.AddWithValue("@CCCD", nv.CCCD);
                    cmd.Parameters.AddWithValue("@DienThoai", nv.DienThoai);
                    cmd.Parameters.AddWithValue("@DiaChi", nv.DiaChi);
                    cmd.Parameters.AddWithValue("@Email", nv.Email);
                    cmd.Parameters.AddWithValue("@HinhAnh", nv.HinhAnh);
                    cmd.Parameters.AddWithValue("@ID_Nhom", nv.ID_VaiTro);
                    cmd.Parameters.AddWithValue("@TrangThai", nv.TrangThai);
                    cmd.Parameters.AddWithValue("@MatKhau", nv.MatKhau);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("SQL Error: " + ex.Message);
            }
        }

        public bool UpdateStaff(Staff nv)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE NHANVIEN SET HoTenNV = @HoTenNV, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh, CCCD = @CCCD, DienThoai = @DienThoai,
                                            DiaChi = @DiaChi, Email = @Email, HinhAnh = @HinhAnh, ID_Nhom = @ID_Nhom, TrangThai = @TrangThai 
                                            WHERE ID_NhanVien = @ID", conn);

                cmd.Parameters.AddWithValue("@HoTenNV", nv.HoTenNV);
                cmd.Parameters.AddWithValue("@NgaySinh", nv.NgaySinh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GioiTinh", nv.GioiTinh);
                cmd.Parameters.AddWithValue("@CCCD", nv.CCCD);
                cmd.Parameters.AddWithValue("@DienThoai", nv.DienThoai);
                cmd.Parameters.AddWithValue("@DiaChi", nv.DiaChi);
                cmd.Parameters.AddWithValue("@Email", nv.Email);
                cmd.Parameters.AddWithValue("@HinhAnh", nv.HinhAnh);
                cmd.Parameters.AddWithValue("@ID_Nhom", nv.ID_VaiTro);
                cmd.Parameters.AddWithValue("@TrangThai", nv.TrangThai);
                cmd.Parameters.AddWithValue("@ID", nv.ID_NhanVien);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteStaff(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM NHANVIEN WHERE ID_NhanVien = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteMultipleStaff(List<int> ids)
        {
            if (ids == null || ids.Count == 0) return false;

            var idParams = string.Join(",", ids.Select((_, i) => $"@id{i}"));

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand($"DELETE FROM NHANVIEN WHERE ID_NhanVien IN ({idParams})", conn);

                for (int i = 0; i < ids.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@id{i}", ids[i]);
                }

                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public Staff GetStaffById(int id)
        {
            Staff staff = null;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM NHANVIEN WHERE ID_NhanVien = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    staff = new Staff
                    {
                        ID_NhanVien = (int)reader["ID_NhanVien"],
                        HoTenNV = reader["HoTenNV"].ToString(),
                        NgaySinh = reader["NgaySinh"] as DateTime?,
                        GioiTinh = reader["GioiTinh"].ToString(),
                        CCCD = reader["CCCD"].ToString(),
                        DienThoai = reader["DienThoai"].ToString(),
                        DiaChi = reader["DiaChi"].ToString(),
                        Email = reader["Email"].ToString(),
                        HinhAnh = reader["HinhAnh"].ToString(),
                        ID_VaiTro = Convert.ToInt32(reader["ID_Nhom"]),
                        TrangThai = reader["TrangThai"].ToString(),
                        MatKhau = reader["MatKhau"].ToString()
                    };
                }
            }
            return staff;
        }

    }
}
