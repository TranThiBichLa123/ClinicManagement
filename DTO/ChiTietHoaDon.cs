using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ChiTietHoaDon
    {
        public string MoTa { get; set; } // Tên thuốc
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; } = 1;
        public decimal ThanhTien => DonGia * SoLuong;

        public bool IsDrug { get; set; } = false; // true nếu là thuốc hoặc dòng tổng tiền thuốc
        public bool IsDrugDetail { get; set; } = false; // true nếu là dòng chi tiết thuốc
        public bool IsVisible { get; set; } = true; // để ẩn/hiện dòng

    }

}
