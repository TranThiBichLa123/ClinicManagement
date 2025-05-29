using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DTO;

namespace DAL
{
    public class ReportAccess : DatabaseAccess
    {
        public List<BaoCaoSuDungThuoc> GetBaoCaoSuDungThuoc(int thang, int nam)
        {
            var list = new List<BaoCaoSuDungThuoc>();
            string query = @"
                SELECT 
                    t.TenThuoc, 
                    d.TenDVT, 
                    b.SoLuongDung, 
                    b.SoLanDung
                FROM BAOCAOSUDUNGTHUOC b
                JOIN THUOC t ON b.ID_Thuoc = t.ID_Thuoc
                JOIN DVT d ON t.ID_DVT = d.ID_DVT
                WHERE b.Thang = @thang AND b.Nam = @nam";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@thang", thang);
                cmd.Parameters.AddWithValue("@nam", nam);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new BaoCaoSuDungThuoc
                    {
                        TenThuoc = reader["TenThuoc"].ToString(),
                        TenDVT = reader["TenDVT"].ToString(),
                        SoLuongDung = (int)reader["SoLuongDung"],
                        SoLanDung = (int)reader["SoLanDung"]
                    });
                }
            }

            return list;
        }


        public BaoCaoDoanhThu GetTongDoanhThu(int thang, int nam)
        {
            BaoCaoDoanhThu result = null;
            string query = @"SELECT * FROM BAOCAODOANHTHU WHERE Thang = @thang AND Nam = @nam";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@thang", thang);
                cmd.Parameters.AddWithValue("@nam", nam);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    result = new BaoCaoDoanhThu
                    {
                        Thang = thang,
                        Nam = nam,
                        TongDoanhThu = (decimal)reader["TongDoanhThu"]
                    };
                }
            }

            return result;
        }


        public List<CT_BaoCaoDoanhThu> GetChiTietDoanhThu(int thang, int nam)
        {
            var list = new List<CT_BaoCaoDoanhThu>();
            string query = @"
            SELECT c.Ngay, c.SoBenhNhan, c.DoanhThu, c.TyLe
            FROM CT_BAOCAODOANHTHU c
            JOIN BAOCAODOANHTHU b ON c.ID_BCDT = b.ID_BCDT
            WHERE b.Thang = @thang AND b.Nam = @nam";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@thang", thang);
                cmd.Parameters.AddWithValue("@nam", nam);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new CT_BaoCaoDoanhThu
                    {
                        Ngay = (int)reader["Ngay"],
                        SoBenhNhan = (int)reader["SoBenhNhan"],
                        DoanhThu = (decimal)reader["DoanhThu"],
                        TyLe = (decimal)reader["TyLe"] 
                    });
                }
            }

            return list;
        }

        public List<ThongKeBenhNhan> GetThongKeBenhNhanTheoNgay()
        {
            List<ThongKeBenhNhan> list = new List<ThongKeBenhNhan>();
            string query = @"
        SELECT NgayTN AS Ngay, COUNT(*) AS SoBenhNhan
        FROM DANHSACHTIEPNHAN
        WHERE Is_Deleted = 0
        GROUP BY NgayTN
        ORDER BY NgayTN
    ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ThongKeBenhNhan
                        {
                            Ngay = Convert.ToDateTime(reader["Ngay"]),
                            SoBenhNhan = Convert.ToInt32(reader["SoBenhNhan"])
                        });
                    }
                }
            }

            return list;
        }
        public List<TopThuoc> GetTopThuocSuDungNhieuNhat()
        {
            var list = new List<TopThuoc>();
            string query = @"
        SELECT TOP 3 
            t.TenThuoc, 
            SUM(b.SoLuongDung) AS TongSoLuongDung
        FROM BAOCAOSUDUNGTHUOC b
        JOIN THUOC t ON b.ID_Thuoc = t.ID_Thuoc
        GROUP BY t.TenThuoc
        ORDER BY TongSoLuongDung DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new TopThuoc
                    {
                        TenThuoc = reader["TenThuoc"].ToString(),
                        TongSoLuongDung = (int)reader["TongSoLuongDung"]
                    });
                }
            }

            return list;
        }

        public List<GioiTinhThongKe> GetThongKeGioiTinh()
        {
            var list = new List<GioiTinhThongKe>();
            string query = @"
        SELECT GioiTinh, COUNT(*) AS SoLuong
        FROM BENHNHAN
        WHERE Is_Deleted = 0
        GROUP BY GioiTinh";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new GioiTinhThongKe
                    {
                        GioiTinh = reader["GioiTinh"].ToString(),
                        SoLuong = (int)reader["SoLuong"]
                    });
                }
            }
            return list;
        }
        public List<DoTuoiThongKe> GetThongKeDoTuoi()
        {
            var list = new List<DoTuoiThongKe>();
            string query = @"
        SELECT 
            CASE 
                WHEN DATEDIFF(YEAR, NgaySinh, GETDATE()) <= 18 THEN '0-18'
                WHEN DATEDIFF(YEAR, NgaySinh, GETDATE()) <= 35 THEN '19-35'
                WHEN DATEDIFF(YEAR, NgaySinh, GETDATE()) <= 60 THEN '36-60'
                ELSE '60+'
            END AS NhomTuoi,
            COUNT(*) AS SoLuong
        FROM BENHNHAN
        WHERE Is_Deleted = 0
        GROUP BY 
            CASE 
                WHEN DATEDIFF(YEAR, NgaySinh, GETDATE()) <= 18 THEN '0-18'
                WHEN DATEDIFF(YEAR, NgaySinh, GETDATE()) <= 35 THEN '19-35'
                WHEN DATEDIFF(YEAR, NgaySinh, GETDATE()) <= 60 THEN '36-60'
                ELSE '60+'
            END";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new DoTuoiThongKe
                    {
                        NhomTuoi = reader["NhomTuoi"].ToString(),
                        SoLuong = (int)reader["SoLuong"]
                    });
                }
            }

            return list;
        }

        public List<TinhThanhThongKe> GetThongKeTinhThanh()
        {
            var list = new List<TinhThanhThongKe>();
            string query = @"
        SELECT DiaChi, COUNT(*) AS SoLuong
        FROM BENHNHAN
        WHERE Is_Deleted = 0
        GROUP BY DiaChi";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new TinhThanhThongKe
                    {
                        DiaChi = reader["DiaChi"].ToString(),
                        SoLuong = (int)reader["SoLuong"]
                    });
                }
            }
            return list;
        }
        public List<DoanhThuTheoThang> GetDoanhThuTheoThang()
        {
            var list = new List<DoanhThuTheoThang>();
            string query = "SELECT Thang, Nam, TongDoanhThu FROM BAOCAODOANHTHU ORDER BY Nam, Thang";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new DoanhThuTheoThang
                    {
                        Thang = (int)reader["Thang"],
                        Nam = (int)reader["Nam"],
                        TongDoanhThu = (decimal)reader["TongDoanhThu"]
                    });
                }
            }

            return list;
        }

        public int GetSoBenhNhanHomNay()
        {
            int count = 0;
            string query = @"
        SELECT COUNT(*) 
        FROM DANHSACHTIEPNHAN 
        WHERE Is_Deleted = 0 
          AND CAST(NgayTN AS DATE) = CAST(GETDATE() AS DATE)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                count = (int)cmd.ExecuteScalar();
            }

            return count;
        }
    }
}
