using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement
{
    public static class UserSession
    {
        public static string Email { get; set; }
        public static int NhomQuyen { get; set; }
        public static List<int> DanhSachChucNang { get; set; } = new List<int>();
    }
}
