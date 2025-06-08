using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class PhanQuyen
    {
        public class NhomNguoiDungDTO
        {
            public int ID_Nhom { get; set; }
            public string TenNhom { get; set; }
        }
        public class ChucNangDTO
        {
            public int ID_ChucNang { get; set; }
            public string TenChucNang { get; set; }
            public string TenManHinhDuocLoad { get; set; }
        }
    }
}
