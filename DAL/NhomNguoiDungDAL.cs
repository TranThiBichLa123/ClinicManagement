using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTO.PhanQuyen;

namespace DAL
{
    public class NhomNguoiDungDAL: DatabaseAccess
    {
        

        public List<NhomNguoiDungDTO> GetAll()
        {
            List<NhomNguoiDungDTO> list = new List<NhomNguoiDungDTO>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_Nhom, TenNhom FROM NHOMNGUOIDUNG";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new NhomNguoiDungDTO
                    {
                        ID_Nhom = reader.GetInt32(0),
                        TenNhom = reader.GetString(1)
                    });
                }
            }

            return list;
        }
    }
}
