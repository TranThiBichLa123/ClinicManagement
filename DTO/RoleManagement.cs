using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class RoleManagement
    {
        public bool IsSelected { get; set; } // Dùng cho checkbox chọn dòng
        public int ID_VaiTro { get; set; }    // Mã nhóm quyền
        public string TenVaiTro { get; set; } // Tên nhóm quyền
    }

    public class AccountViewModel
    {
        public int ID_TaiKhoan { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public int ID_VaiTro { get; set; }
        public string TrangThai { get; set; } // "Đang làm" / "Đã thôi việc"
                                              // Convert trạng thái thành "Hoạt động" hoặc "Vô hiệu hóa"
        public string TrangThaiTaiKhoan => TrangThai == "Đang làm việc" ? "Hoạt động" : "Vô hiệu hóa";

        public bool IsDisabled => TrangThai == "Đã thôi việc";
        public bool IsSelected { get; set; } // Cho checkbox nếu muốn thao tác hàng loạt
    }
}
