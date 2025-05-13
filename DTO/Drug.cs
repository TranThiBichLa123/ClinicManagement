using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Drug
    {
        public int ID_Thuoc { get; set; }
        public string TenThuoc { get; set; }
        public string TenDVT { get; set; }
        public string CachDung { get; set; }
        public string ThanhPhan { get; set; }
        public string XuatXu { get; set; }
        public int SoLuongTon { get; set; }
        public double DonGiaNhap { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HinhAnh { get; set; }
        public decimal? TyLeGiaBan { get; set; }
        public double DonGiaBan { get; set; }
        public DateTime? NgayNhap { get; set; }

    }
    public class ThongKeTuanDTO
    {
        public int Tuan { get; set; }         // Số thứ tự tuần
        public int DaBan { get; set; }        // Tổng số lượng đã bán trong tuần
        public int TonKho { get; set; }       // Tổng tồn kho trong tuần (nếu bạn có dùng)

        // Có thể thêm property này nếu cần ghép nhãn biểu đồ
        public string Label => $"Tuần {Tuan}";
    }

}
