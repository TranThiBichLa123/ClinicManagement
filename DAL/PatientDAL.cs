using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PatientDAL : DatabaseAccess
    {
        public DataTable LoadPatientList(string nameKeyword = "", string idKeyword = "", string loaiBenhKeyword = "", string trieuChungKeyword = "", string ngayDK = "")
        {
            string query = @"
                SELECT 
                    BN.ID_BenhNhan,
                    BN.HoTenBN,
                    BN.NgaySinh,
                    BN.GioiTinh,
                    BN.CCCD,
                    BN.DienThoai,
                    BN.DiaChi,
                    BN.Email,
                    BN.NgayDK
                FROM BENHNHAN BN
                WHERE BN.Is_Deleted = 0
                ";

            if (!string.IsNullOrEmpty(loaiBenhKeyword) || !string.IsNullOrEmpty(trieuChungKeyword))
            {
                query += @"
                    AND EXISTS (
                        SELECT 1
                        FROM PHIEUKHAM PK
                        JOIN DANHSACHTIEPNHAN TN ON TN.ID_TiepNhan = PK.ID_TiepNhan
                        JOIN LOAIBENH LB ON PK.ID_LoaiBenh = LB.ID_LoaiBenh
                        WHERE TN.ID_BenhNhan = BN.ID_BenhNhan
                ";

                if (!string.IsNullOrEmpty(loaiBenhKeyword))
                    query += " AND LB.TenLoaiBenh LIKE @loaiBenhKeyword";

                if (!string.IsNullOrEmpty(trieuChungKeyword))
                    query += " AND PK.TrieuChung LIKE @trieuChungKeyword";

                query += ")";
            }

            if (!string.IsNullOrEmpty(nameKeyword))
                query += " AND BN.HoTenBN LIKE @nameKeyword";
            if (!string.IsNullOrEmpty(idKeyword))
                query += " AND CAST(BN.ID_BenhNhan AS NVARCHAR) LIKE @idKeyword";
            if (!string.IsNullOrEmpty(ngayDK))
                query += " AND CONVERT(VARCHAR, BN.NgayDK, 103) LIKE @ngayDK";

            using (SqlConnection conn = (SqlConnection)CreateConnection())
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nameKeyword", "%" + nameKeyword + "%");
                cmd.Parameters.AddWithValue("@idKeyword", "%" + idKeyword + "%");
                cmd.Parameters.AddWithValue("@loaiBenhKeyword", "%" + loaiBenhKeyword + "%");
                cmd.Parameters.AddWithValue("@trieuChungKeyword", "%" + trieuChungKeyword + "%");
                cmd.Parameters.AddWithValue("@ngayDK", "%" + ngayDK + "%");

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
        public bool AddPatientToDatabase(BenhNhan patient)
        {
            string query = @"
                INSERT INTO BenhNhan (HoTenBN, NgaySinh, GioiTinh, CCCD, DienThoai, DiaChi, Email, NgayDK, Is_Deleted)
                VALUES (@HoTenBN, @NgaySinh, @GioiTinh, @CCCD, @DienThoai, @DiaChi, @Email, @NgayDK, @Is_Deleted)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@HoTenBN", patient.HoTenBN);
                cmd.Parameters.AddWithValue("@NgaySinh", patient.NgaySinh);
                cmd.Parameters.AddWithValue("@GioiTinh", patient.GioiTinh);
                cmd.Parameters.AddWithValue("@CCCD", patient.CCCD);
                cmd.Parameters.AddWithValue("@DienThoai", patient.DienThoai);
                cmd.Parameters.AddWithValue("@DiaChi", patient.DiaChi);
                cmd.Parameters.AddWithValue("@Email", patient.Email);
                cmd.Parameters.AddWithValue("@NgayDK", patient.NgayDK);
                cmd.Parameters.AddWithValue("@Is_Deleted", patient.Is_Deleted);

                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }
        public bool UpdatePatient(BenhNhan bn)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE BENHNHAN SET HoTenBN = @HoTenBN, NgaySinh = @NgaySinh, GioiTinh = @GioiTinh, 
                                 CCCD = @CCCD, DienThoai = @DienThoai, DiaChi = @DiaChi, Email = @Email 
                                 WHERE ID_BenhNhan = @ID_BenhNhan";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HoTenBN", bn.HoTenBN);
                cmd.Parameters.AddWithValue("@NgaySinh", bn.NgaySinh);
                cmd.Parameters.AddWithValue("@GioiTinh", bn.GioiTinh);
                cmd.Parameters.AddWithValue("@CCCD", bn.CCCD);
                cmd.Parameters.AddWithValue("@DienThoai", bn.DienThoai);
                cmd.Parameters.AddWithValue("@DiaChi", bn.DiaChi);
                cmd.Parameters.AddWithValue("@Email", bn.Email);
                cmd.Parameters.AddWithValue("@ID_BenhNhan", bn.ID_BenhNhan);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Return codes: 0 - không tìm thấy, 1 - xóa thành công, 2 - có phiếu khám
        public int DeletePatient(int idBenhNhan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                                    UPDATE BENHNHAN 
                                    SET Is_Deleted = 1 
                                    WHERE ID_BenhNhan = @ID
                                    AND NOT EXISTS (
                                        SELECT 1
                                        FROM DANHSACHTIEPNHAN TN
                                        JOIN PHIEUKHAM PK ON PK.ID_TiepNhan = TN.ID_TiepNhan
                                        WHERE TN.ID_BenhNhan = @ID
                                    )";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", idBenhNhan);

                conn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result > 0) return 1;

                // Kiểm tra xem có tồn tại bệnh nhân không
                string checkQuery = "SELECT COUNT(*) FROM BENHNHAN WHERE ID_BenhNhan = @ID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@ID", idBenhNhan);
                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                    return 2; // tồn tại nhưng có phiếu khám
                else
                    return 0; // không tìm thấy
            }
        }

    }
}
