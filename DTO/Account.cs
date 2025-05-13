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
        public string TrangThai { get; set; }
    }
}
