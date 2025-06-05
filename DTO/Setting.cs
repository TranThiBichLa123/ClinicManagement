﻿using System;
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

    /*public class Staff
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
    */

    public class QuiDinh
    {
        public string TenQuiDinh { get; set; }
        public double GiaTri { get; set; }
    }

    public class DonViTinh
    {
        public int ID_DVT { get; set; }
        public string TenDVT { get; set; }
    }

  
    public class CachDung
    {
        public int ID_CachDung { get; set; }      
        public string MoTaCachDung { get; set; }
    }

    public class LoaiBenh
    {
        public int ID_LoaiBenh { get; set; }
        public string TenLoaiBenh { get; set; }
        public string TrieuChung { get; set; }
        public string HuongDieuTri { get; set; }
    }

}
