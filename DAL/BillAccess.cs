using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using DTO;

namespace DAL
{
    public class BillAccess : DatabaseAccess
    {
        public void CapNhatBaoCaoSauKhiTaoHoaDon(int thang, int nam)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Xóa báo cáo doanh thu
                var delCT = new SqlCommand("DELETE FROM CT_BAOCAODOANHTHU WHERE ID_BCDT IN (SELECT ID_BCDT FROM BAOCAODOANHTHU WHERE Thang = @Thang AND Nam = @Nam)", conn);
                delCT.Parameters.AddWithValue("@Thang", thang);
                delCT.Parameters.AddWithValue("@Nam", nam);
                delCT.ExecuteNonQuery();

                var delMain = new SqlCommand("DELETE FROM BAOCAODOANHTHU WHERE Thang = @Thang AND Nam = @Nam", conn);
                delMain.Parameters.AddWithValue("@Thang", thang);
                delMain.Parameters.AddWithValue("@Nam", nam);
                delMain.ExecuteNonQuery();

                // Xóa báo cáo sử dụng thuốc
                var delThuoc = new SqlCommand("DELETE FROM BAOCAOSUDUNGTHUOC WHERE Thang = @Thang AND Nam = @Nam", conn);
                delThuoc.Parameters.AddWithValue("@Thang", thang);
                delThuoc.Parameters.AddWithValue("@Nam", nam);
                delThuoc.ExecuteNonQuery();

                // Tạo lại
                var exec1 = new SqlCommand("EXEC TaoBaoCaoDoanhThu @Thang, @Nam", conn);
                exec1.Parameters.AddWithValue("@Thang", thang);
                exec1.Parameters.AddWithValue("@Nam", nam);
                exec1.ExecuteNonQuery();

                var exec2 = new SqlCommand("EXEC TaoBaoCaoSuDungThuoc @Thang, @Nam", conn);
                exec2.Parameters.AddWithValue("@Thang", thang);
                exec2.Parameters.AddWithValue("@Nam", nam);
                exec2.ExecuteNonQuery();
            }
        }
        public int InsertHoaDon(int idPhieuKham, int idNhanVien, DateTime ngay)
        {
            int idHoaDon = GetHoaDonIdByPhieuKham(idPhieuKham);
            if (idHoaDon != 0)
                return idHoaDon;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand(@"
                INSERT INTO HOADON (ID_PhieuKham, ID_NhanVien, NgayHoaDon)
                VALUES (@idPK, @idNV, @ngay);
                SELECT SCOPE_IDENTITY();", conn);

                    cmd.Parameters.AddWithValue("@idPK", idPhieuKham);
                    cmd.Parameters.AddWithValue("@idNV", idNhanVien);
                    cmd.Parameters.AddWithValue("@ngay", ngay);

                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int id))
                    {
                        idHoaDon = id;
                    }
                }
                catch (SqlException ex) when (ex.Message.Contains("FOREIGN KEY constraint") && ex.Message.Contains("FK_HOADON_ID_Phieu"))
                {
                    // Ném lỗi nghiệp vụ rõ ràng, không bị coi là lỗi hệ thống
                    throw new ApplicationException("Mã phiếu khám không tồn tại trong hệ thống. Không thể lập hóa đơn.");
                }
                catch (Exception ex)
                {
                    // Ném lỗi chung khác
                    throw new Exception("Đã xảy ra lỗi khi tạo hóa đơn: " + ex.Message);
                }
            }

            return idHoaDon;
        }


        private int GetHoaDonIdByPhieuKham(int idPhieuKham)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 ID_HoaDon FROM HOADON WHERE ID_PhieuKham = @id", conn);
                cmd.Parameters.AddWithValue("@id", idPhieuKham);
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }


        public HoaDon GetThongTinHoaDon(int idPhieuKham)
        {
            HoaDon hd = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 HD.ID_HoaDon, HD.NgayHoaDon, HD.TongTien, HD.ID_NhanVien,
       BN.HoTenBN AS TenBenhNhan

                FROM HOADON HD
                JOIN PHIEUKHAM PK ON HD.ID_PhieuKham = PK.ID_PhieuKham
                JOIN DANHSACHTIEPNHAN TN ON PK.ID_TiepNhan = TN.ID_TiepNhan
                JOIN BENHNHAN BN ON TN.ID_BenhNhan = BN.ID_BenhNhan
                WHERE HD.ID_PhieuKham = @idPK", conn);

                cmd.Parameters.AddWithValue("@idPK", idPhieuKham);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    hd = new HoaDon
                    {
                        MaHoaDon = (int)reader["ID_HoaDon"],
                        NgayLap = (DateTime)reader["NgayHoaDon"],
                        TenBenhNhan = reader["TenBenhNhan"].ToString(),
                        MaNhanVien = (int)reader["ID_NhanVien"],
                        TongTien = (decimal)reader["TongTien"]
                    };
                }
            }
            return hd;
        }

        public List<ChiTietHoaDon> GetChiTietHoaDon(int idPhieuKham)
        {
            var list = new List<ChiTietHoaDon>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Tiền khám
                var tienKhamCmd = new SqlCommand("SELECT TienKham FROM PHIEUKHAM WHERE ID_PhieuKham = @idPK", conn);
                tienKhamCmd.Parameters.AddWithValue("@idPK", idPhieuKham);
                object tienKham = tienKhamCmd.ExecuteScalar();
                if (tienKham != null)
                {
                    list.Add(new ChiTietHoaDon
                    {
                        MoTa = "Tiền khám",
                        DonGia = (decimal)tienKham,
                        SoLuong = 1
                    });
                }

                // Tiền thuốc và chi tiết
                var cmd = new SqlCommand(@"
            SELECT T.TenThuoc, TT.DonGiaBan_LucMua, TT.SoLuong
            FROM TOATHUOC TT
            JOIN THUOC T ON TT.ID_Thuoc = T.ID_Thuoc
            WHERE TT.ID_PhieuKham = @idPK", conn);
                cmd.Parameters.AddWithValue("@idPK", idPhieuKham);

                using (var reader = cmd.ExecuteReader())
                {
                    decimal tongTienThuoc = 0;
                    var chiTietThuoc = new List<ChiTietHoaDon>();

                    while (reader.Read())
                    {
                        var donGia = (decimal)reader["DonGiaBan_LucMua"];
                        var soLuong = (int)reader["SoLuong"];
                        tongTienThuoc += donGia * soLuong;

                        chiTietThuoc.Add(new ChiTietHoaDon
                        {
                            MoTa = reader["TenThuoc"].ToString(),
                            DonGia = donGia,
                            SoLuong = soLuong,
                            IsDrug = true,
                            IsDrugDetail = true,
                            IsVisible = false // mặc định ẩn
                        });
                    }

                    if (tongTienThuoc > 0)
                    {
                        list.Add(new ChiTietHoaDon
                        {
                            MoTa = "Tiền thuốc",
                            DonGia = tongTienThuoc,
                            SoLuong = 1,
                            IsDrug = true
                        });

                        list.AddRange(chiTietThuoc);
                    }
                }
            }

            return list;
        }


        public List<NhanVien_ThuNgan> GetThuNganDangLam()
        {
            var list = new List<NhanVien_ThuNgan>();
            string query = @"
        SELECT NV.ID_NhanVien, NV.HoTenNV
        FROM NHANVIEN NV
        JOIN NHOMNGUOIDUNG NND ON NV.ID_Nhom = NND.ID_Nhom
        WHERE NV.TrangThai = N'Đang làm việc' AND NND.TenNhom = N'Thu ngân'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new NhanVien_ThuNgan
                    {
                        ID_NhanVien = (int)reader["ID_NhanVien"],
                        HoTenNV = reader["HoTenNV"].ToString()
                    });
                }
            }

            return list;
        }
        public bool CapNhatHoaDon(int idPhieuKham, int idNhanVien, DateTime ngay)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE HOADON SET ID_NhanVien = @idNV, NgayHoaDon = @ngay WHERE ID_PhieuKham = @idPK", conn);
                cmd.Parameters.AddWithValue("@idNV", idNhanVien);
                cmd.Parameters.AddWithValue("@ngay", ngay);
                cmd.Parameters.AddWithValue("@idPK", idPhieuKham);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaHoaDon(int idPhieuKham)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM HOADON WHERE ID_PhieuKham = @idPK", conn);
                cmd.Parameters.AddWithValue("@idPK", idPhieuKham);
                return cmd.ExecuteNonQuery() > 0;
            }
        }



        public List<HoaDon> GetDanhSachHoaDon()
        {
            var list = new List<HoaDon>();

            string query = @"
        SELECT HD.ID_HoaDon, HD.ID_PhieuKham, HD.ID_NhanVien, HD.NgayHoaDon,
               PK.TienKham, 
               ISNULL(SUM(TT.SoLuong * TT.DonGiaBan_LucMua), 0) AS TienThuoc,
               HD.TongTien
        FROM HOADON HD
        JOIN PHIEUKHAM PK ON HD.ID_PhieuKham = PK.ID_PhieuKham
        LEFT JOIN TOATHUOC TT ON PK.ID_PhieuKham = TT.ID_PhieuKham
        GROUP BY HD.ID_HoaDon, HD.ID_PhieuKham, HD.ID_NhanVien, HD.NgayHoaDon, PK.TienKham, HD.TongTien
        ORDER BY HD.NgayHoaDon DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new HoaDon
                    {
                        MaHoaDon = (int)reader["ID_HoaDon"],
                        MaPhieuKham = (int)reader["ID_PhieuKham"],
                        MaNhanVien = (int)reader["ID_NhanVien"],
                        NgayLap = (DateTime)reader["NgayHoaDon"],
                        TienKham = (decimal)reader["TienKham"],
                        TienThuoc = (decimal)reader["TienThuoc"],
                        TongTien = (decimal)reader["TongTien"]
                    });
                }
            }

            return list;
        }

        public List<HoaDon> GetDanhSachHoaDonTheoNhanVien(int idNhanVien)
        {
            var list = new List<HoaDon>();

            string query = @"
        SELECT HD.ID_HoaDon, HD.ID_PhieuKham, HD.ID_NhanVien, HD.NgayHoaDon,
               PK.TienKham,
               ISNULL(SUM(TT.SoLuong * TT.DonGiaBan_LucMua), 0) AS TienThuoc,
               HD.TongTien
        FROM HOADON HD
        JOIN PHIEUKHAM PK ON HD.ID_PhieuKham = PK.ID_PhieuKham
        LEFT JOIN TOATHUOC TT ON PK.ID_PhieuKham = TT.ID_PhieuKham
        WHERE HD.ID_NhanVien = @idNV
        GROUP BY HD.ID_HoaDon, HD.ID_PhieuKham, HD.ID_NhanVien, HD.NgayHoaDon, PK.TienKham, HD.TongTien
        ORDER BY HD.NgayHoaDon DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idNV", idNhanVien);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new HoaDon
                    {
                        MaHoaDon = (int)reader["ID_HoaDon"],
                        MaPhieuKham = (int)reader["ID_PhieuKham"],
                        MaNhanVien = (int)reader["ID_NhanVien"],
                        NgayLap = (DateTime)reader["NgayHoaDon"],
                        TienKham = (decimal)reader["TienKham"],
                        TienThuoc = (decimal)reader["TienThuoc"],
                        TongTien = (decimal)reader["TongTien"]
                    });
                }
            }

            return list;
        }

    }

}
