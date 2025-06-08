using System;

namespace DTO
{
    public class NhapThuoc
    {
        public class ThuocDTO
        {
            public int ID_Thuoc { get; set; }
            public string TenThuoc { get; set; }
            public int ID_DVT { get; set; }
            public int ID_CachDung { get; set; }
            public string ThanhPhan { get; set; }
            public string XuatXu { get; set; }
            public int SoLuongTon { get; set; } = 0;
            public decimal DonGiaNhap { get; set; }
            public decimal? DonGiaBan { get; set; }
            public decimal TyLeGiaBan { get; set; }
            public string HinhAnh { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class PhieuNhapThuocDTO
        {
            public int ID_PhieuNhapThuoc { get; set; }
            public DateTime NgayNhap { get; set; }
            public decimal TongTienNhap { get; set; } = 0;
        }

        public class ChiTietPhieuNhapThuocDTO
        {
            public int ID_PhieuNhapThuoc { get; set; }
            public int ID_Thuoc { get; set; }
            public string TenThuoc { get; set; }

            public DateTime? HanSuDung { get; set; }
            public int SoLuongNhap { get; set; }
            public decimal DonGiaNhap { get; set; }
            public decimal ThanhTien { get; set; }

            public string HinhAnh { get; set; }

            public int ID_DVT { get; set; }
            public int ID_CachDung { get; set; }
            public string ThanhPhan { get; set; }
            public string XuatXu { get; set; }
            public decimal TyLeGiaBan { get; set; }
        }


        public class DonViTinhDTO
        {
            public int ID_DVT { get; set; }
            public string TenDVT { get; set; }
        }

        public class CachDungDTO
        {
            public int ID_CachDung { get; set; }
            public string MoTaCachDung { get; set; }
        }
    }
}
