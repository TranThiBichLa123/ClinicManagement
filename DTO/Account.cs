using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Account
    {
        public int ID_TaiKhoan { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public int ID_VaiTro { get; set; }
        public bool TrangThai { get; set; }

        public Account() { }

        public Account(int id, string email, string matKhau, int idVaiTro, bool trangThai)
        {
            ID_TaiKhoan = id;
            Email = email;
            MatKhau = matKhau;
            ID_VaiTro = idVaiTro;
            TrangThai = trangThai;
        }
    }

    public class BenhNhan
    {
        public int ID_BenhNhan { get; set; }
        public string HoTenBN { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string CCCD { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public DateTime NgayDK { get; set; }
        public bool Is_Deleted { get; set; }
    }

    public class PhieuKham
    {
        public int ID_PhieuKham { get; set; }
        public DateTime? NgayTN { get; set; }
        public string CaTN { get; set; }
        public string TienKham { get; set; }
        public string TongTienThuoc { get; set; }
    }
    public class Thuoc
    {
        public int ID_PhieuKham { get; set; }
        public string TenThuoc { get; set; }
        public int SoLuong { get; set; }
        public string TienThuoc { get; set; }  
        public string MoTaCachDung { get; set; }
    }
}
