using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClinicManagement
{
    public class PhanQuyenHelper
    {
        // Danh sách quyền của user hiện tại
        public static List<int> DanhSachQuyen { get; set; }

        // Kiểm tra quyền UI
        public static Visibility KiemTraQuyenUI(int idChucNang)
        {
            return DanhSachQuyen.Contains(idChucNang) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Kiểm tra quyền Logic
        public static bool KiemTraQuyenLogic(int idChucNang)
        {
            if (!DanhSachQuyen.Contains(idChucNang))
                throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng này");
            return true;
        }

        // Kiểm tra quyền Data
        public static IQueryable<T> LocDuLieuTheoQuyen<T>(IQueryable<T> data, int userId) where T : IPhanQuyenData
        {
            return data.Where(x => x.UserId == userId);
        }
    }

    public interface IPhanQuyenData
    {
        int UserId { get; set; }
    }
}
