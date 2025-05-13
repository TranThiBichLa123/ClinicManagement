using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
namespace DAL
{
    public class NewDrugAccess : DatabaseAccess
    {
        public NewDrugAccess() : base() { }

        public List<string> GetDanhSachTenThuoc()
        {
            List<string> list = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT TenThuoc FROM Thuoc", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader["TenThuoc"].ToString());
                }
            }
            return list;
        }
        public List<string> GetDsNgayNhap()
        {
            List<string> list = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT NgayNhap FROM PHIEUNHAPTHUOC", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Đọc DateTime và chuyển sang chuỗi định dạng ngắn (yyyy-MM-dd)
                    DateTime ngayNhap = reader.GetDateTime(reader.GetOrdinal("NgayNhap"));
                    list.Add(ngayNhap.ToString("yyyy-MM-dd"));
                }
            }
            return list;
        }


        public List<string> GetDanhSachThanhPhan()
        {
            List<string> list = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT ThanhPhan FROM Thuoc", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader["ThanhPhan"].ToString());
                }
            }
            return list;
        }


        public List<string> GetDanhSachXuatXu()
        {
            List<string> list = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT XuatXu FROM Thuoc", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader["XuatXu"].ToString());
                }
            }
            return list;
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

        public bool InsertDanhSachThuoc(List<NewDrug> danhSach)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string insertPhieu = "INSERT INTO PHIEUNHAPTHUOC (NgayNhap) VALUES (@NgayNhap); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdPhieu = new SqlCommand(insertPhieu, conn, transaction);
                    cmdPhieu.Parameters.AddWithValue("@NgayNhap", DateTime.Now);
                    int idPhieu = Convert.ToInt32(cmdPhieu.ExecuteScalar());

                    foreach (var newDrug in danhSach)
                    {
                        int idDVT = GetID_DVTByTen(newDrug.TenDVT);
                        int idCachDung = GetID_CachDungByMoTa(newDrug.MoTaCachDung);

                        string checkThuoc = @"
SELECT ID_Thuoc 
FROM THUOC 
WHERE TenThuoc = @TenThuoc AND ID_DVT = @ID_DVT 
AND ID_CachDung = @ID_CachDung 
AND ThanhPhan = @ThanhPhan 
AND XuatXu = @XuatXu 
AND DonGiaNhap = @DonGiaNhap 
AND TyLeGiaBan = @TyLeGiaBan 
AND IsDeleted = 0";

                        SqlCommand checkCmd = new SqlCommand(checkThuoc, conn, transaction);
                        checkCmd.Parameters.AddWithValue("@TenThuoc", newDrug.TenThuoc);
                        checkCmd.Parameters.AddWithValue("@ID_DVT", idDVT);
                        checkCmd.Parameters.AddWithValue("@ID_CachDung", idCachDung);
                        checkCmd.Parameters.AddWithValue("@ThanhPhan", newDrug.ThanhPhan);
                        checkCmd.Parameters.AddWithValue("@XuatXu", newDrug.XuatXu);
                        checkCmd.Parameters.AddWithValue("@DonGiaNhap", newDrug.DonGiaNhap);
                        checkCmd.Parameters.AddWithValue("@TyLeGiaBan", newDrug.TyLeGiaBan);

                        object result = checkCmd.ExecuteScalar();
                        int idThuoc;

                        if (result != null)
                        {
                            idThuoc = Convert.ToInt32(result);
                        }
                        else
                        {
                            string insertThuoc = @"
INSERT INTO THUOC (TenThuoc, ID_DVT, ID_CachDung, ThanhPhan, XuatXu, DonGiaNhap, HanSuDung, HinhAnh, TyLeGiaBan)
VALUES (@TenThuoc, @ID_DVT, @ID_CachDung, @ThanhPhan, @XuatXu, @DonGiaNhap, @HanSuDung, @HinhAnh, @TyLeGiaBan);
SELECT SCOPE_IDENTITY();";

                            SqlCommand cmdThuoc = new SqlCommand(insertThuoc, conn, transaction);
                            cmdThuoc.Parameters.AddWithValue("@TenThuoc", newDrug.TenThuoc);
                            cmdThuoc.Parameters.AddWithValue("@ID_DVT", idDVT);
                            cmdThuoc.Parameters.AddWithValue("@ID_CachDung", idCachDung);
                            cmdThuoc.Parameters.AddWithValue("@ThanhPhan", newDrug.ThanhPhan);
                            cmdThuoc.Parameters.AddWithValue("@XuatXu", newDrug.XuatXu);
                            cmdThuoc.Parameters.AddWithValue("@DonGiaNhap", newDrug.DonGiaNhap);
                            cmdThuoc.Parameters.AddWithValue("@HanSuDung", newDrug.HanSuDung);
                            cmdThuoc.Parameters.AddWithValue("@HinhAnh", newDrug.HinhAnh ?? (object)DBNull.Value);
                            cmdThuoc.Parameters.AddWithValue("@TyLeGiaBan", newDrug.TyLeGiaBan);

                            idThuoc = Convert.ToInt32(cmdThuoc.ExecuteScalar());
                        }

                        string insertChiTiet = @"
INSERT INTO CHITIETPHIEUNHAPTHUOC (ID_PhieuNhapThuoc, ID_Thuoc, SoLuongNhap, DonGiaNhap, HanSuDung)
VALUES (@ID_PhieuNhapThuoc, @ID_Thuoc, @SoLuongNhap, @DonGiaNhap, @HanSuDung);";

                        SqlCommand cmdChiTiet = new SqlCommand(insertChiTiet, conn, transaction);
                        cmdChiTiet.Parameters.AddWithValue("@ID_PhieuNhapThuoc", idPhieu);
                        cmdChiTiet.Parameters.AddWithValue("@ID_Thuoc", idThuoc);
                        cmdChiTiet.Parameters.AddWithValue("@SoLuongNhap", newDrug.SoLuongNhap);
                        cmdChiTiet.Parameters.AddWithValue("@DonGiaNhap", newDrug.DonGiaNhap);
                        cmdChiTiet.Parameters.AddWithValue("@HanSuDung", newDrug.HanSuDung); 
                        cmdChiTiet.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Lỗi khi thêm danh sách thuốc: " + ex.Message);
                }
            }
        }
       
    } 
}















