using System;
namespace DTO
{
    public class Setting
    {
        public int ID_VaiTro { get; set; }
        public string TenVaiTro { get; set; }
   /*
    * public Setting() { }

        public Setting(int id, string ten)
        {
            ID_VaiTro = id;
            TenVaiTro = ten;
        }

        public override bool Equals(object obj)
        {
            return obj is Setting setting &&
                   ID_VaiTro == setting.ID_VaiTro &&
                   TenVaiTro == setting.TenVaiTro;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID_VaiTro, TenVaiTro);
        }

        public override string ToString()
        {
            return $"Setting {{ ID_VaiTro = {ID_VaiTro}, TenVaiTro = {TenVaiTro} }}";
        }
   */
    }

    public class Staff
    {
        public int ID_VaiTro { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public int ID_NhanVien { get; set; }
        public string HoTenNV { set; get; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string CCCD { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }
        public string TrangThai { get; set; }
        public string HinhAnh { get; set; }
    }

}
