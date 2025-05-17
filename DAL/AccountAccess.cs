using System.Data.SqlClient;
using System.Data;
using DTO;
using System.Security.Principal;
using System;
using System.Windows;
using System.Collections.Generic;
namespace DAL
{
    public class AccountAccess : DatabaseAccess
    {
        public DataTable GetUserWithRole(Account acc)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("proc_login", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Thêm tham số cho stored procedure
                cmd.Parameters.AddWithValue("@user", acc.Email);  // Tên tài khoản
                cmd.Parameters.AddWithValue("@pass", acc.MatKhau);    // Mật khẩu

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                return dataTable;
            }
        }

        public string CheckLogin(Account taikhoan, out string userRole)
        {
            userRole = string.Empty; // Khởi tạo giá trị quyền mặc định

            if (string.IsNullOrEmpty(taikhoan.Email))
            {
                return "request_taikhoan"; // Trả về lỗi nếu tài khoản trống
            }
            if (string.IsNullOrEmpty(taikhoan.MatKhau))
            {
                return "request_password"; // Trả về lỗi nếu mật khẩu trống
            }

            // Lấy dữ liệu từ cơ sở dữ liệu và kiểm tra
            DataTable result = GetUserWithRole(taikhoan);
            if (result.Rows.Count > 0)
            {
                // Lấy quyền của người dùng từ kết quả trả về
                userRole = result.Rows[0]["sTenQuyen"].ToString();
                return "success"; // Đăng nhập thành công
            }

            return "invalid_login"; // Nếu không tìm thấy người dùng
        }

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

                using (SqlConnection conn = CreateConnection())
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
        public class ExaminationDAL : DatabaseAccess
        {
            public BenhNhan GetBenhNhanById(string id)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM BENHNHAN WHERE ID_BenhNhan = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new BenhNhan
                        {
                            ID_BenhNhan = Convert.ToInt32(reader["ID_BenhNhan"]),
                            HoTenBN = reader["HoTenBN"].ToString(),
                            NgaySinh = Convert.ToDateTime(reader["NgaySinh"]),
                            GioiTinh = reader["GioiTinh"].ToString()
                        };
                    }

                    return null;
                }
            }
            public List<PhieuKham> LoadPhieuKham(string idBenhNhan)
            {
                List<PhieuKham> danhSachPhieu = new List<PhieuKham>();
                string query = @"SELECT ID_PhieuKham, TN.NgayTN, TN.CaTN, TienKham, TongTienThuoc 
                     FROM PHIEUKHAM PK
                     JOIN DANHSACHTIEPNHAN TN ON TN.ID_TiepNhan=PK.ID_TiepNhan
                     WHERE TN.ID_BenhNhan = @ID_BenhNhan";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID_BenhNhan", idBenhNhan);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        danhSachPhieu.Add(new PhieuKham
                        {
                            ID_PhieuKham = reader.GetInt32(0),
                            NgayTN = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1),
                            CaTN = reader.IsDBNull(2) ? "Không rõ" : reader.GetString(2), // Ca khám sáng/chiều
                            TienKham = reader.IsDBNull(3) ? "0" : reader.GetDecimal(3).ToString("N0"),
                            TongTienThuoc = reader.IsDBNull(4) ? "0" : reader.GetDecimal(4).ToString("N0")
                        });
                    }
                }

                return danhSachPhieu;
            }
            public List<Thuoc> LoadDanhSachThuoc(int idPhieuKham)
            {
                List<Thuoc> danhSachThuoc = new List<Thuoc>();
                string query = @"SELECT PK.ID_PhieuKham, TenThuoc, SoLuong, TienThuoc, MoTaCachDung
                                            FROM PHIEUKHAM PK 
                                            JOIN TOATHUOC CT ON PK.ID_PhieuKham = CT.ID_PhieuKham 
                                            JOIN THUOC T ON CT.ID_Thuoc = T.ID_Thuoc 
                                            JOIN CACHDUNG C ON C.ID_CachDung = T.ID_CachDung
                                            WHERE PK.ID_PhieuKham = @ID_PhieuKham";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        danhSachThuoc.Add(new Thuoc
                        {
                            ID_PhieuKham = reader.GetInt32(reader.GetOrdinal("ID_PhieuKham")),
                            TenThuoc = reader["TenThuoc"].ToString(),
                            SoLuong = reader.GetInt32(reader.GetOrdinal("SoLuong")),
                            TienThuoc = reader.IsDBNull(reader.GetOrdinal("TienThuoc"))
                                ? "Chưa có"
                                : Convert.ToDecimal(reader["TienThuoc"]).ToString("N0"),
                            MoTaCachDung = reader["MoTaCachDung"].ToString()
                        });
                    }
                }
                return danhSachThuoc;
            }
            public object[] ShowExaminationPopup(int idPhieuKham)
            {
                string querry = @"SELECT HoTenBN, PK.TrieuChung, TenLoaiBenh, ID_PhieuKham, TienKham, TongTienThuoc, PK.CAKham
                                  FROM PHIEUKHAM PK 
                                  JOIN DANHSACHTIEPNHAN TN ON TN.ID_TiepNhan=PK.ID_TiepNhan
                                  JOIN BENHNHAN BN ON BN.ID_BenhNhan = TN.ID_BenhNhan 
                                  JOIN LOAIBENH LB ON PK.ID_LoaiBenh = LB.ID_LoaiBenh
                                  WHERE PK.ID_PhieuKham = @ID_PhieuKham
                                ";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(querry, con);
                    cmd.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);

                    SqlDataReader reader = cmd.ExecuteReader();
                    string hoTenBN = null;
                    string trieuChung = null;
                    string tenLoaiBenh = null;
                    decimal tienKham = 0;
                    decimal tongTienThuoc = 0;
                    string caKham = null;
                    while (reader.Read())
                    {
                        hoTenBN = reader["HoTenBN"].ToString();
                        trieuChung = reader["TrieuChung"].ToString();
                        tenLoaiBenh = reader["TenLoaiBenh"].ToString();
                        caKham = reader["CAKham"].ToString();
                        tienKham = (decimal)reader["TienKham"];
                        if (reader["TongTienThuoc"] != DBNull.Value)
                            tongTienThuoc = (decimal)reader["TongTienThuoc"];
                        else tongTienThuoc = 0;
                    }
                    reader.Close();
                    return new object[] { hoTenBN, trieuChung, tenLoaiBenh, tienKham, tongTienThuoc, caKham };
                }               
            }
        }
    }
}
