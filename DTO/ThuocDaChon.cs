using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ThuocDaChon
    {
        public string ID_Thuoc { get; set; }
        public decimal DonGiaBan { get; set; }
        public string MoTa { get; set; }
        public string DonViTinh { get; set; }
        public string TenThuoc { get; set; }
        public int SoLuong { get; set; } = 1;
    }
}
