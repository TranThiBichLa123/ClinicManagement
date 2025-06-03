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
    public class ExaminationFormViewDAL : DatabaseAccess
    {
        public DataRow GetPhieuKham(int idTN)
        {
            string query = $@"SELECT HoTenBN, PK.TrieuChung, TenLoaiBenh, ID_PhieuKham, TienKham, TongTienThuoc, CaKham, NgayTN     
                                  FROM PHIEUKHAM PK JOIN DANHSACHTIEPNHAN TN ON PK.ID_TiepNhan = TN.ID_TiepNhan
                                                    JOIN BENHNHAN BN ON BN.ID_BenhNhan = TN.ID_BenhNhan 
                                                    JOIN LOAIBENH LB ON PK.ID_LoaiBenh = LB.ID_LoaiBenh
                                  WHERE PK.ID_TiepNhan = {idTN}";
            var dt = GetData(query);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataTable GetToaThuocTheoPhieuKham(int idPK)
        {
            string query = $@"SELECT TenThuoc, TenDVT, SoLuong, MoTaCachDung, DonGiaBan_LucMua, TienThuoc
                                       FROM PHIEUKHAM PK JOIN TOATHUOC CT ON PK.ID_PhieuKham = CT.ID_PhieuKham JOIN THUOC T ON CT.ID_Thuoc = T.ID_Thuoc
                                                        JOIN DVT ON DVT.ID_DVT = T.ID_DVT JOIN CACHDUNG C ON C.ID_CachDung = T.ID_CachDUng
                                       WHERE PK.ID_PhieuKham = {idPK}";
            return GetData(query);
        }

        public void XoaPhieuKham(int idPK)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"UPDATE PHIEUKHAM SET Is_Deleted = 1
                                 WHERE ID_PhieuKham = @IDPK";

                SqlCommand cmd = new SqlCommand(query, conn);
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

        public bool CheckDaXuatHD(int idPK)
        {
            string query = $@"SELECT * FROM HOADON WHERE ID_PhieuKham = {idPK}";
            var dt = GetData(query);
            return dt.Rows.Count > 0 ? true : false;
        }
    }
}
