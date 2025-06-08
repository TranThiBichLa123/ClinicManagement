using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTO;
using static DTO.NhapThuoc;

namespace DAL
{
    public class NhapThuocAccess : DatabaseAccess
    {
        // Lấy danh sách tên thuốc (cho ComboBox)
        public List<string> GetDanhSachTenThuoc()
        {
            var list = new List<string>();
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT TenThuoc FROM THUOC WHERE IsDeleted = 0", (SqlConnection)conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
            }
            return list;
        }

        // Lấy thông tin thuốc theo tên
        public ThuocDTO GetThuocByTen(string tenThuoc)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM THUOC WHERE TenThuoc = @TenThuoc AND IsDeleted = 0", (SqlConnection)conn);
                cmd.Parameters.AddWithValue("@TenThuoc", tenThuoc);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new ThuocDTO
                    {
                        ID_Thuoc = Convert.ToInt32(reader["ID_Thuoc"]),
                        TenThuoc = reader["TenThuoc"].ToString(),
                        ID_DVT = Convert.ToInt32(reader["ID_DVT"]),
                        ID_CachDung = Convert.ToInt32(reader["ID_CachDung"]),
                        ThanhPhan = reader["ThanhPhan"].ToString(),
                        XuatXu = reader["XuatXu"].ToString(),
                        SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]),
                        DonGiaNhap = Convert.ToDecimal(reader["DonGiaNhap"]),
                        DonGiaBan = reader["DonGiaBan"] != DBNull.Value
    ? (decimal?)Convert.ToDecimal(reader["DonGiaBan"])
    : null,

                        TyLeGiaBan = Convert.ToDecimal(reader["TyLeGiaBan"]),
                        HinhAnh = reader["HinhAnh"].ToString(),
                        IsDeleted = Convert.ToBoolean(reader["IsDeleted"])
                    };
                }
            }
            return null;
        }

        // Thêm mới thuốc nếu chưa tồn tại
        public int InsertThuoc(ThuocDTO thuoc)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO THUOC (TenThuoc, ID_DVT, ID_CachDung, ThanhPhan, XuatXu, SoLuongTon, DonGiaNhap, TyLeGiaBan, HinhAnh)
                    VALUES (@TenThuoc, @ID_DVT, @ID_CachDung, @ThanhPhan, @XuatXu, 0, @DonGiaNhap, @TyLeGiaBan, @HinhAnh);
                    SELECT SCOPE_IDENTITY();", (SqlConnection)conn);

                cmd.Parameters.AddWithValue("@TenThuoc", thuoc.TenThuoc);
                cmd.Parameters.AddWithValue("@ID_DVT", thuoc.ID_DVT);
                cmd.Parameters.AddWithValue("@ID_CachDung", thuoc.ID_CachDung);
                cmd.Parameters.AddWithValue("@ThanhPhan", (object)thuoc.ThanhPhan ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@XuatXu", (object)thuoc.XuatXu ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DonGiaNhap", thuoc.DonGiaNhap);
                cmd.Parameters.AddWithValue("@TyLeGiaBan", thuoc.TyLeGiaBan);
                cmd.Parameters.AddWithValue("@HinhAnh", (object)thuoc.HinhAnh ?? DBNull.Value);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void UpdateThuocAndIncreaseQuantity(ThuocDTO thuoc, int soLuongThem)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    UPDATE THUOC SET
                        SoLuongTon = SoLuongTon + @SoLuongThem,
                        DonGiaNhap = @DonGiaNhap,
                        ThanhPhan = @ThanhPhan,
                        XuatXu = @XuatXu,
                        TyLeGiaBan = @TyLeGiaBan,
                        HinhAnh = @HinhAnh
                    WHERE ID_Thuoc = @ID_Thuoc", (SqlConnection)conn);

                cmd.Parameters.AddWithValue("@SoLuongThem", soLuongThem);
                cmd.Parameters.AddWithValue("@DonGiaNhap", thuoc.DonGiaNhap);
                cmd.Parameters.AddWithValue("@ThanhPhan", (object)thuoc.ThanhPhan ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@XuatXu", (object)thuoc.XuatXu ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TyLeGiaBan", thuoc.TyLeGiaBan);
                cmd.Parameters.AddWithValue("@HinhAnh", (object)thuoc.HinhAnh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_Thuoc", thuoc.ID_Thuoc);

                cmd.ExecuteNonQuery();
            }
        }

        public int CreatePhieuNhapThuoc(DateTime ngayNhap)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO PHIEUNHAPTHUOC (NgayNhap) VALUES (@NgayNhap); SELECT SCOPE_IDENTITY();", (SqlConnection)conn);
                cmd.Parameters.AddWithValue("@NgayNhap", ngayNhap);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void InsertChiTietPhieuNhap(ChiTietPhieuNhapThuocDTO ct)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO CHITIETPHIEUNHAPTHUOC (ID_PhieuNhapThuoc, ID_Thuoc, HanSuDung, SoLuongNhap, DonGiaNhap) VALUES (@ID_Phieu, @ID_Thuoc, @HanSuDung, @SoLuong, @DonGia)", (SqlConnection)conn);
                cmd.Parameters.AddWithValue("@ID_Phieu", ct.ID_PhieuNhapThuoc);
                cmd.Parameters.AddWithValue("@ID_Thuoc", ct.ID_Thuoc);
                cmd.Parameters.AddWithValue("@HanSuDung", (object)ct.HanSuDung ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SoLuong", ct.SoLuongNhap);
                cmd.Parameters.AddWithValue("@DonGia", ct.DonGiaNhap);
                cmd.ExecuteNonQuery();
            }
        }

        public List<PhieuNhapThuocDTO> GetAllPhieuNhap()
        {
            var list = new List<PhieuNhapThuocDTO>();
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ID_PhieuNhapThuoc, NgayNhap, TongTienNhap FROM PHIEUNHAPTHUOC", (SqlConnection)conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new PhieuNhapThuocDTO
                    {
                        ID_PhieuNhapThuoc = reader.GetInt32(0),
                        NgayNhap = reader.GetDateTime(1),
                        TongTienNhap = reader.GetDecimal(2)
                    });
                }
            }
            return list;
        }

        public List<ChiTietPhieuNhapThuocDTO> GetChiTietPhieuNhap(int idPhieu)
        {
            var list = new List<ChiTietPhieuNhapThuocDTO>();
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT c.ID_Thuoc, t.TenThuoc, c.HanSuDung, c.SoLuongNhap, c.DonGiaNhap, c.ThanhTien
                                           FROM CHITIETPHIEUNHAPTHUOC c
                                           JOIN THUOC t ON c.ID_Thuoc = t.ID_Thuoc
                                           WHERE c.ID_PhieuNhapThuoc = @ID", (SqlConnection)conn);
                cmd.Parameters.AddWithValue("@ID", idPhieu);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ChiTietPhieuNhapThuocDTO
                    {
                        ID_Thuoc = Convert.ToInt32(reader["ID_Thuoc"]),
                        TenThuoc = reader["TenThuoc"].ToString(),
                        HanSuDung = reader["HanSuDung"] as DateTime?,
                        SoLuongNhap = Convert.ToInt32(reader["SoLuongNhap"]),
                        DonGiaNhap = Convert.ToDecimal(reader["DonGiaNhap"]),
                        ThanhTien = Convert.ToDecimal(reader["ThanhTien"])
                    });
                }
            }
            return list;
        }

        public List<DonViTinhDTO> GetAllDVT()
        {
            var list = new List<DonViTinhDTO>();
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ID_DVT, TenDVT FROM DVT", (SqlConnection)conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new DonViTinhDTO
                    {
                        ID_DVT = reader.GetInt32(0),
                        TenDVT = reader.GetString(1)
                    });
                }
            }
            return list;
        }

        public List<CachDungDTO> GetAllCachDung()
        {
            var list = new List<CachDungDTO>();
            using (var conn = CreateConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ID_CachDung, MoTaCachDung FROM CACHDUNG", (SqlConnection)conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new CachDungDTO
                    {
                        ID_CachDung = reader.GetInt32(0),
                        MoTaCachDung = reader.GetString(1)
                    });
                }
            }
            return list;
        }
    }
}
