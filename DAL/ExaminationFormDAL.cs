using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class ExaminationFormDAL : DatabaseAccess
    {
        public DataTable GetLoaiBenh() => GetData("SELECT ID_LoaiBenh, TenLoaiBenh FROM LOAIBENH");

        public DataTable GetThuocList()
        {
            string query = @"SELECT T.ID_Thuoc, T.TenThuoc, T.SoLuongTon, T.DonGiaBan, C.MoTaCachDung, T.ID_DVT, D.TenDVT, T.HinhAnh
                             FROM THUOC T 
                             JOIN DVT D ON T.ID_DVT = D.ID_DVT 
                             JOIN CACHDUNG C ON C.ID_CachDung = T.ID_CachDung";
            return GetData(query);
        }

        public DataRow GetThongTinBenhNhan(int idBN)
        {
            string query = $"SELECT HoTenBN FROM BENHNHAN WHERE ID_BenhNhan = {idBN}";
            var dt = GetData(query);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DateTime? GetNgayTiepNhan(int idTiepNhan)
        {
            string query = @"SELECT NgayTN FROM DANHSACHTIEPNHAN WHERE ID_TiepNhan = {idTiepNhan}";
            var dt = GetData(query);
            return dt.Rows.Count > 0 ? dt.Rows[0].Field<DateTime>("NgayTN") : (DateTime?)null;
        }

        public DataRow GetPhieuKham(int idPK)
        {
            string query = $@"SELECT TN.ID_BenhNhan, HoTenBN, TrieuChung, ID_LoaiBenh, CaKham, NgayTN 
                              FROM PHIEUKHAM PK 
                              JOIN DANHSACHTIEPNHAN TN ON PK.ID_TiepNhan = TN.ID_TiepNhan
                              JOIN BENHNHAN BN ON BN.ID_BenhNhan = TN.ID_BenhNhan
                              WHERE ID_PhieuKham = {idPK} AND PK.Is_Deleted = 0";
            var dt = GetData(query);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataTable GetToaThuocTheoPhieuKham(int idPK)
        {
            string query = $@"SELECT CT.ID_Thuoc, TenThuoc, TenDVT, SoLuong, MoTaCachDung, DonGiaBan_LucMua, TienThuoc
                              FROM TOATHUOC CT
                              JOIN THUOC T ON CT.ID_Thuoc = T.ID_Thuoc
                              JOIN DVT ON DVT.ID_DVT = T.ID_DVT
                              JOIN CACHDUNG C ON C.ID_CachDung = T.ID_CachDung
                              WHERE ID_PhieuKham = {idPK}";
            return GetData(query);
        }

        public DataRow GetChiTietThuoc(string idThuoc)
        {
            string query = $@"SELECT TenDVT, MoTaCachDung, DonGiaBan FROM THUOC
                              JOIN DVT ON THUOC.ID_DVT = DVT.ID_DVT
                              JOIN CACHDUNG ON THUOC.ID_CachDung = CACHDUNG.ID_CachDung
                              WHERE ID_Thuoc = '{idThuoc}'";
            var dt = GetData(query);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public int InsertPhieuKham(int idTiepNhan, string caKham, string trieuChung, int idLoaiBenh)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO PHIEUKHAM (ID_TiepNhan, CaKham, TrieuChung, ID_LoaiBenh)
                                 VALUES (@ID_TN, @CK, @TC, @LB);
                                 SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID_TN", idTiepNhan);
                cmd.Parameters.AddWithValue("@CK", caKham);
                cmd.Parameters.AddWithValue("@TC", trieuChung);
                cmd.Parameters.AddWithValue("@LB", idLoaiBenh);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void UpdatePhieuKham(int idPK, string caKham, string trieuChung, int idLoaiBenh)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"UPDATE PHIEUKHAM 
                                 SET CaKham = @CK, TrieuChung = @TC, ID_LoaiBenh = @LB 
                                 WHERE ID_PhieuKham = @IDPK";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CK", caKham);
                cmd.Parameters.AddWithValue("@TC", trieuChung);
                cmd.Parameters.AddWithValue("@LB", idLoaiBenh);
                cmd.Parameters.AddWithValue("@IDPK", idPK);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteToaThuoc(int idPK)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM TOATHUOC WHERE ID_PhieuKham = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", idPK);
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertToaThuoc(int idPK, ThuocDaChon thuoc)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"INSERT INTO TOATHUOC (ID_PhieuKham, ID_Thuoc, DonGiaBan_LucMua, SoLuong)
                                                  VALUES (@IDPK, @IDT, @DGB, @SL)", conn);
                cmd.Parameters.AddWithValue("@IDPK", idPK);
                cmd.Parameters.AddWithValue("@IDT", thuoc.ID_Thuoc);
                cmd.Parameters.AddWithValue("@DGB", thuoc.DonGiaBan);
                cmd.Parameters.AddWithValue("@SL", thuoc.SoLuong);
                cmd.ExecuteNonQuery();
            }
        }
    }

}
