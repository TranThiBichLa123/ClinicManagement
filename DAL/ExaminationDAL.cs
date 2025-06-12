using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
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
                     WHERE TN.ID_BenhNhan = @ID_BenhNhan AND PK.Is_Deleted = 0";

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
