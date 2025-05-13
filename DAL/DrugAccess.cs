using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using DTO;
namespace DAL
{
    public class DrugAccess:DatabaseAccess
    {
        public List<Drug> GetDrugList()
        {
            List<Drug> drugList = new List<Drug>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
SELECT 
    t.ID_Thuoc,
    t.TenThuoc,
    t.ID_DVT,
    d.TenDVT,
    t.ID_CachDung,
    cd.MoTaCachDung AS CachDung,
    t.ThanhPhan,
    t.XuatXu,
    t.SoLuongTon,
    t.DonGiaNhap,
    t.HinhAnh,
    t.TyLeGiaBan,
    t.DonGiaBan
FROM thuoc t
JOIN cachdung cd ON t.ID_CachDung = cd.ID_CachDung
JOIN dvt d ON t.ID_DVT = d.ID_DVT
WHERE t.IsDeleted = 0
";
              
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Drug drug = new Drug
                    {       ID_Thuoc= Convert.ToInt32(reader["ID_Thuoc"]),
                        TenThuoc = reader["TenThuoc"].ToString(),
                        CachDung = reader["CachDung"].ToString(),
                        TenDVT = reader["TenDVT"].ToString(),
                        ThanhPhan = reader["ThanhPhan"].ToString(),
                        XuatXu = reader["XuatXu"].ToString(),
                        SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]),
                        DonGiaNhap = Convert.ToDouble(reader["DonGiaNhap"]),
                        HinhAnh = reader["HinhAnh"].ToString(),

                        TyLeGiaBan = reader["TyLeGiaBan"] != DBNull.Value  ? Convert.ToDecimal(reader["TyLeGiaBan"]): (decimal?)null,

                        DonGiaBan = Convert.ToDouble(reader["DonGiaBan"])
     
                    };
                    drugList.Add(drug);
                }
                reader.Close();
            }
            return drugList;
        }

        public async Task<List<Drug>> SearchAsync1(string keyword)
        {
            var list = new List<Drug>();
     using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string query = @"
SELECT 
    t.ID_Thuoc,
    t.TenThuoc,
    t.ID_DVT,
    d.TenDVT,
    t.ID_CachDung,
    cd.MoTaCachDung AS CachDung,
    t.ThanhPhan,
    t.XuatXu,
    t.SoLuongTon,
    t.DonGiaNhap,
    t.HinhAnh,
    t.TyLeGiaBan,
    t.DonGiaBan
FROM thuoc t
JOIN cachdung cd ON t.ID_CachDung = cd.ID_CachDung
JOIN dvt d ON t.ID_DVT = d.ID_DVT
WHERE (@TENTHUOC IS NULL OR t.TenThuoc LIKE '%' + @TENTHUOC + '%')
";


                using (var cmd = new SqlCommand(query, conn))
                {
                    object paramValue = string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : keyword;
                    cmd.Parameters.AddWithValue("@TENTHUOC", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Drug
                            {
                                ID_Thuoc = Convert.ToInt32(reader["ID_Thuoc"]),
                                TenThuoc = reader["TenThuoc"].ToString(),
                                TenDVT = reader["TenDVT"].ToString(),
                                CachDung = reader["CachDung"].ToString(),
                                ThanhPhan = reader["ThanhPhan"].ToString(),
                                XuatXu = reader["XuatXu"].ToString(),
                                SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]),
                                DonGiaNhap = Convert.ToDouble(reader["DonGiaNhap"]),
                                HinhAnh = reader["HinhAnh"].ToString(),
                                TyLeGiaBan = reader["TyLeGiaBan"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["TyLeGiaBan"]),
                                DonGiaBan = Convert.ToDouble(reader["DonGiaBan"])
                            });
                        }
                    }
                }
            }

            return list;
        }


        public async Task<List<Drug>> SearchAsync2(string keyword) 
        {
            var list = new List<Drug>();
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string query = @"
SELECT 
    t.ID_Thuoc,
    t.TenThuoc,
    t.ID_DVT,
    d.TenDVT,
    t.ID_CachDung,
    cd.MoTaCachDung AS CachDung,
    t.ThanhPhan,
    t.XuatXu,
    t.SoLuongTon,
    t.DonGiaNhap,
    t.HinhAnh,
    t.TyLeGiaBan,
    t.DonGiaBan
FROM thuoc t
JOIN cachdung cd ON t.ID_CachDung = cd.ID_CachDung
JOIN dvt d ON t.ID_DVT = d.ID_DVT
WHERE 
 (@ThanhPhan IS NULL OR t.ThanhPhan LIKE '%' + @ThanhPhan + '%')
 
";


                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ThanhPhan", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Drug
                            {
                                ID_Thuoc = Convert.ToInt32(reader["ID_Thuoc"]),
                                TenThuoc = reader["TenThuoc"].ToString(),
                                TenDVT = reader["TenDVT"].ToString(),
                                CachDung = reader["CachDung"].ToString(),
                                ThanhPhan = reader["ThanhPhan"].ToString(),
                                XuatXu = reader["XuatXu"].ToString(),
                                SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]),
                                DonGiaNhap = Convert.ToDouble(reader["DonGiaNhap"]),
                                HinhAnh = reader["HinhAnh"].ToString(),
                                TyLeGiaBan = reader["TyLeGiaBan"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["TyLeGiaBan"]),
                                DonGiaBan = Convert.ToDouble(reader["DonGiaBan"])


                            });
                        }
                    }
                }
            }
            return list;
        }


        public async Task<List<Drug>> SearchAsync3(string keyword) 
        {
            var list = new List<Drug>();
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string query = @"
SELECT 
    t.ID_Thuoc,
    t.TenThuoc,
    t.ID_DVT,
    d.TenDVT,
    t.ID_CachDung,
    cd.MoTaCachDung AS CachDung,
    t.ThanhPhan,
    t.XuatXu,
    t.SoLuongTon,
    t.DonGiaNhap,
    t.HinhAnh,
    t.TyLeGiaBan,
    t.DonGiaBan
FROM thuoc t
JOIN cachdung cd ON t.ID_CachDung = cd.ID_CachDung
JOIN dvt d ON t.ID_DVT = d.ID_DVT
WHERE (@HanSuDung IS NULL OR t.HanSuDung = @HanSuDung)
";


                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@HanSuDung", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Drug
                            {
                                ID_Thuoc = Convert.ToInt32(reader["ID_Thuoc"]),
                                TenThuoc = reader["TenThuoc"].ToString(),
                                TenDVT = reader["TenDVT"].ToString(),
                                CachDung = reader["CachDung"].ToString(),
                                ThanhPhan = reader["ThanhPhan"].ToString(),
                                XuatXu = reader["XuatXu"].ToString(),
                                SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]),
                                DonGiaNhap = Convert.ToDouble(reader["DonGiaNhap"]),
                                HinhAnh = reader["HinhAnh"].ToString(),
                                TyLeGiaBan = reader["TyLeGiaBan"] == DBNull.Value ? null : (decimal?)Convert.ToDecimal(reader["TyLeGiaBan"]),
                                DonGiaBan = Convert.ToDouble(reader["DonGiaBan"])


                            });
                        }
                    }
                }
            }
            return list;
        }

        public bool DeleteDrug(int idThuoc)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE THUOC SET IsDeleted = 1 WHERE ID_Thuoc = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", idThuoc);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public Drug GetDrugByTen(string tenThuoc)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT TOP 1 
                t.ID_Thuoc, 
                t.TenThuoc, 
                d.TenDVT, 
                c.MoTaCachDung, 
                t.ThanhPhan, 
                t.XuatXu, 
                t.DonGiaNhap, 
                t.TyLeGiaBan, 
                t.HinhAnh
            FROM THUOC t
            JOIN DVT d ON t.ID_DVT = d.ID_DVT
            JOIN CACHDUNG c ON t.ID_CachDung = c.ID_CachDung
            WHERE t.TenThuoc = @TenThuoc AND t.IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenThuoc", tenThuoc);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Drug
                        {
                            ID_Thuoc = (int)reader["ID_Thuoc"],
                            TenThuoc = reader["TenThuoc"].ToString(),
                            TenDVT = reader["TenDVT"].ToString(),
                            CachDung = reader["MoTaCachDung"].ToString(),
                            ThanhPhan = reader["ThanhPhan"].ToString(),
                            XuatXu = reader["XuatXu"].ToString(),
                            DonGiaNhap = Convert.ToDouble(reader["DonGiaNhap"]),
                            TyLeGiaBan = Convert.ToDecimal(reader["TyLeGiaBan"]),
                            HinhAnh = reader["HinhAnh"] != DBNull.Value ? reader["HinhAnh"].ToString() : ""
                        };
                    }
                }
            }
            return null;
        }
        public Drug GetHsdByNgayNhap(string ngayNhap)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT TOP 1 
                p.NgayNhap, 
                c.HanSuDung
            FROM PHIEUNHAPTHUOC p
            JOIN CHITIETPHIEUNHAPTHUOC c ON c.ID_PhieuNhapThuoc = p.ID_PhieuNhapThuoc
            JOIN THUOC t ON t.ID_Thuoc = c.ID_Thuoc
            WHERE p.NgayNhap = @NgayNhap AND t.IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(query, conn);

                // ✅ Convert string to DateTime to match SQL type
                if (DateTime.TryParse(ngayNhap, out DateTime parsedDate))
                {
                    cmd.Parameters.Add("@NgayNhap", SqlDbType.Date).Value = parsedDate;
                }
                else
                {
                    return null; // invalid input
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Drug
                        {
                            HanSuDung = reader.GetDateTime(reader.GetOrdinal("HanSuDung"))
                        };
                    }
                }
            }

            return null;
        }


        public int GetID_DVTByTen(string TenDVT)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_DVT FROM DVT WHERE TenDVT = @TenDVT";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenDVT", TenDVT);
                return (int?)cmd.ExecuteScalar() ?? -1;
            }
        }
        public int GetID_CachDungByMoTa(string MoTaCachDung)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_CachDung FROM CachDung WHERE MoTaCachDung = @MoTaCachDung";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MoTaCachDung", MoTaCachDung);
                return (int?)cmd.ExecuteScalar() ?? -1;
            }
        }
        public bool UpdateDrug(Drug drug)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
UPDATE THUOC
SET 
    TenThuoc = @TenThuoc,
    ID_DVT = @ID_DVT,
    ID_CachDung = @ID_CachDung,
    ThanhPhan = @ThanhPhan,
    XuatXu = @XuatXu,
    DonGiaNhap = @DonGiaNhap,
    TyLeGiaBan = @TyLeGiaBan,
    HinhAnh = @HinhAnh
WHERE ID_Thuoc = @ID_Thuoc
";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID_Thuoc", drug.ID_Thuoc);
                cmd.Parameters.AddWithValue("@TenThuoc", drug.TenThuoc);
                cmd.Parameters.AddWithValue("@ThanhPhan", drug.ThanhPhan);
                cmd.Parameters.AddWithValue("@XuatXu", drug.XuatXu);
                cmd.Parameters.AddWithValue("@DonGiaNhap", drug.DonGiaNhap);
                cmd.Parameters.AddWithValue("@TyLeGiaBan", drug.TyLeGiaBan);
                cmd.Parameters.AddWithValue("@HinhAnh", drug.HinhAnh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_DVT", GetID_DVTByTen(drug.TenDVT));
                cmd.Parameters.AddWithValue("@ID_CachDung", GetID_CachDungByMoTa(drug.CachDung));

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        /*
                public List<string> GetAllDonViTinh()
                {
                    List<string> list = new List<string>();
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT TenDVT FROM DVT ";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            list.Add(reader["TenDVT"].ToString());
                        }
                    }
                    return list;
                }

                public List<string> GetAllCachDung()
                {
                    List<string> list = new List<string>();
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT MoTaCachDung FROM CACHDUNG";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            list.Add(reader["MoTaCachDung"].ToString());
                        }
                    }
                    return list;
                }
        */
        public int GetTongSoLuongDaBan(int month, int year)
        {
            int total = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT SUM(TongSoLuong)
            FROM BAOCAOSUDUNGTHUOC
            WHERE Thang = @Month AND Nam = @Year";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);
                var result = cmd.ExecuteScalar();
                total = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
            return total;
        }

        public int GetTongSoLuongTonKho(int month, int year)
        {
            int total = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT SUM(SoLuongTon) 
            FROM THUOC 
            WHERE 
                IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(query, conn);
               /* cmd.Parameters.AddWithValue("@THANG", month);
                cmd.Parameters.AddWithValue("@NAM", year);*/
                var result = cmd.ExecuteScalar();
                total = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
            return total;
        }

        public List<ThongKeTuanDTO> GetThongKeTheoTuan(int month, int year)
        {
            List<ThongKeTuanDTO> list = new List<ThongKeTuanDTO>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {  
                conn.Open();

                string query = @"
    SELECT 
        DENSE_RANK() OVER (ORDER BY DATEPART(WEEK, Ngay)) AS Tuan,
        SUM(TongSoLuong) AS DaBan,
        0 AS TonKho
    FROM BAOCAOSUDUNGTHUOC
    WHERE MONTH(Ngay) = @THANG AND YEAR(Ngay) = @NAM
    GROUP BY DATEPART(WEEK, Ngay)
    ORDER BY Tuan
";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@THANG", month);
                cmd.Parameters.AddWithValue("@NAM", year);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ThongKeTuanDTO
                        {
                            Tuan = reader.GetInt32(0),
                            DaBan = reader.GetInt32(1)
                        });
                    }
                }
            }
            return list;
        }

    }
}
