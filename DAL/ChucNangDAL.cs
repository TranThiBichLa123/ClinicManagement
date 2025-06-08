using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTO.PhanQuyen;

namespace DAL
{
    public class ChucNangDAL:DatabaseAccess
    {
        

        public List<ChucNangDTO> GetAll()
        {
            var list = new List<ChucNangDTO>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_ChucNang, TenChucNang, TenManHinhDuocLoad FROM CHUCNANG";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new ChucNangDTO
                    {
                        ID_ChucNang = reader.GetInt32(0),
                        TenChucNang = reader.GetString(1),
                        TenManHinhDuocLoad = reader.IsDBNull(2) ? null : reader.GetString(2)
                    });
                }
            }

            return list;
        }
    }
}
