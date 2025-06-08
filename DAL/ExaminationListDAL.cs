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
    public class ExaminationListDAL : DatabaseAccess
    {
        public DataTable GetDanhSachTiepNhan(DateTime ngayKham)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT BN.ID_BenhNhan, BN.HoTenBN, CAST(BN.NgaySinh AS DATE) AS NgaySinh, BN.GioiTinh, TN.ID_TiepNhan, TN.NgayTN, TN.CaTN, TN.ID_NhanVien
                                FROM DANHSACHTIEPNHAN TN JOIN BENHNHAN BN ON TN.ID_BenhNhan = BN.ID_BenhNhan
                                WHERE TN.Is_Deleted = 0 AND TN.NgayTN = @NgayKham";
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                adapter.SelectCommand.Parameters.AddWithValue("@NgayKham", ngayKham.Date);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
        public DataTable GetDanhSachTiepNhanTheoNhanVien(DateTime ngay, int idNhanVien)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT BN.ID_BenhNhan, BN.HoTenBN, CAST(BN.NgaySinh AS DATE) AS NgaySinh, BN.GioiTinh,
                                TN.ID_TiepNhan, TN.NgayTN, TN.CaTN, TN.ID_NhanVien
                         FROM DANHSACHTIEPNHAN TN
                         JOIN BENHNHAN BN ON TN.ID_BenhNhan = BN.ID_BenhNhan
                         WHERE TN.Is_Deleted = 0 AND TN.NgayTN = @NgayKham AND TN.ID_NhanVien = @ID_NV";

                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                adapter.SelectCommand.Parameters.AddWithValue("@NgayKham", ngay.Date);
                adapter.SelectCommand.Parameters.AddWithValue("@ID_NV", idNhanVien);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public int GetSLBNMax()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT GiaTri FROM QUI_DINH WHERE TenQuiDinh = 'SoLuongTiepNhanToiDa'";
                SqlCommand cmd = new SqlCommand(query, con);
                var result = cmd.ExecuteScalar();
                int sl = Convert.ToInt32(result);
                return sl;
            }
        }
        public int GetSLBNTrongNgay(DateTime ngayKham)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                                 SELECT COUNT(*)
                                FROM DANHSACHTIEPNHAN TN JOIN BENHNHAN BN ON TN.ID_BenhNhan = BN.ID_BenhNhan
                                WHERE TN.Is_Deleted = 0 AND TN.NgayTN = @NgayKham";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@NgayKham", ngayKham.Date);
                var result = cmd.ExecuteScalar();
                int sl = Convert.ToInt32(result);
                return sl;
            }
        }
        public bool CheckDaCoPK(int idTN)
        {
            string query = $@"SELECT * FROM PHIEUKHAM WHERE ID_TiepNhan = {idTN} AND Is_Deleted = 0";
            var dt = GetData(query);
            return dt.Rows.Count > 0 ? true : false;
        }
        public int InsertTiepNhan(string idBN, string idNV, DateTime ngayTN, string caTN)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO DANHSACHTIEPNHAN (ID_BenhNhan, ID_NhanVien, NgayTN, CaTN) 
                                    VALUES (@ID_BenhNhan, @ID_NhanVien, @NgayTN, @CaTN)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID_BenhNhan", idBN);
                cmd.Parameters.AddWithValue("@ID_NhanVien", idNV);
                cmd.Parameters.AddWithValue("@NgayTN", ngayTN);
                cmd.Parameters.AddWithValue("@CaTN", caTN);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
        }
        public int UpdateTiepNhan(string idBN, string idNV, DateTime ngayTN, string caTN, DateTime editNgayTiepNhan, string editCaTN, string editIdBenhNhan, string editIDNhanVien)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"UPDATE DANHSACHTIEPNHAN SET ID_BenhNhan = @ID_BenhNhan, ID_NhanVien = @ID_NhanVien, NgayTN = @NgayTN, CaTN = @CaTN 
                                    WHERE ID_BenhNhan = @editIdBenhNhan AND NgayTN = @editNgayTN AND CaTN = @editCaTN AND ID_NhanVien = @editIDNhanVien";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID_BenhNhan", idBN);
                cmd.Parameters.AddWithValue("@ID_NhanVien", idNV);
                cmd.Parameters.AddWithValue("@CaTN", caTN);
                cmd.Parameters.AddWithValue("@NgayTN", ngayTN);
                cmd.Parameters.AddWithValue("@editNgayTN", editNgayTiepNhan);
                cmd.Parameters.AddWithValue("@editCaTN", editCaTN);
                cmd.Parameters.AddWithValue("@editIdBenhNhan", editIdBenhNhan);
                cmd.Parameters.AddWithValue("@editIDNhanVien", editIDNhanVien);

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected;
            }
        }
        public int DeleteTiepNhan(string delIdBenhNhan, DateTime delNgayTiepNhan, string delCaTN, string delIDNhanVien)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"UPDATE DANHSACHTIEPNHAN SET Is_Deleted = 1 WHERE ID_BenhNhan = @ID_BenhNhan AND NgayTN = @NgayTN
                                    AND CaTN = @CaTN AND ID_NhanVien = @IDNhanVien";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID_BenhNhan", delIdBenhNhan);
                cmd.Parameters.AddWithValue("@NgayTN", delNgayTiepNhan);
                cmd.Parameters.AddWithValue("@CaTN", delCaTN);
                cmd.Parameters.AddWithValue("@IDNhanVien", delIDNhanVien);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
