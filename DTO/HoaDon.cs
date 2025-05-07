using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class HoaDon
    {
      
            public int MaHoaDon { get; set; }
            public int MaPhieuKham { get; set; }
            public int MaNhanVien { get; set; }
            public DateTime NgayLap { get; set; }

            public decimal TienKham { get; set; }
            public decimal TienThuoc { get; set; }
            public decimal TongTien { get; set; }

            public string TenBenhNhan { get; set; }  // Dùng cho CreateBill
       
    }

}
