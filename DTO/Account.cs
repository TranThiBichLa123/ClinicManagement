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

        // Constructor không tham số
        public Account() { }

        // Constructor có tham số (tùy chọn)
        public Account(int id, string email, string matKhau, int idVaiTro, bool trangThai)
        {
            ID_TaiKhoan = id;
            Email = email;
            MatKhau = matKhau;
            ID_VaiTro = idVaiTro;
            TrangThai = trangThai;
        }
    }
}
