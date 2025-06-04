using System;
using System.ComponentModel;

namespace DTO
{
    public class StaffAccount
    {
        public int ID_VaiTro { get; set; }
        public string TenVaiTro { get; set; }
    }

    public class Staff : INotifyPropertyChanged
    {
        public int ID_VaiTro { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public int ID_NhanVien { get; set; }
        public string HoTenNV { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string CCCD { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }
        public string TrangThai { get; set; }
        public string HinhAnh { get; set; }

        public string Mat_Khau { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AccountManager
    {
        public int ID_TaiKhoan { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public int ID_VaiTro { get; set; }
        public string TrangThai { get; set; }
    }
}
