using System;
    namespace DTO
    {
        public class NewDrug
        {
        public string ID_Thuoc { get; set;
        }
            public string TenThuoc { get; set; }
            public string TenDVT { get; set; }
            public string MoTaCachDung { get; set; }
            public string ThanhPhan { get; set; }
            public string XuatXu { get; set; }
            public int SoLuongNhap { get; set; } // trong bảng chi tiết phiếu nhập thuốc
            public double DonGiaNhap { get; set; }
            public decimal TyLeGiaBan { get; set; }
            public DateTime HanSuDung { get; set; }
            public string HinhAnh { get; set; }
        }


}


