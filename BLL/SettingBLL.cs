using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using DAL;
namespace BLL
{
    public class SettingBLL
    {
        private SettingAccess settingAccess = new SettingAccess();

        public List<DTO.Setting> GetRoleList()
        {
            return settingAccess.GetRoleList();
        }
       
        public List<DTO.Account> GetAccountList()
        {
            return settingAccess.GetAccountList();
        }
        public List<DTO.Staff> GetStaffList()
        {
            return settingAccess.GetStaffList();
        }

        // ======================== THÊM CHỨC NĂNG ========================
        /*

        // Thêm vai trò mới
        public bool AddRole(string roleName)
        {
            Setting newRole = new Setting
            {
                ID_VaiTro = settingAccess.GetNextRoleId(), // Hàm tự tăng ID
                TenVaiTro = roleName
            };
            return settingAccess.InsertRole(newRole);
        }

        // Cập nhật vai trò
        public bool UpdateRole(Setting role)
        {
            return settingAccess.UpdateRole(role);
        }

        // Xóa vai trò
        public bool DeleteRole(int idVaiTro)
        {
            return settingAccess.DeleteRole(idVaiTro);
        }

        // Tìm kiếm vai trò theo tên hoặc mã
        public List<Setting> SearchRoles(string keyword)
        {
            var all = GetRoleList();
            return all.Where(r =>
                r.ID_VaiTro.ToString().Contains(keyword) ||
                (r.TenVaiTro != null && r.TenVaiTro.ToLower().Contains(keyword.ToLower()))
            ).ToList();
        }

        // Kiểm tra quyền (ví dụ: dùng khi đăng nhập)
        public bool CheckPermission(int roleId, string chucNang, string hanhDong)
        {
            var chiTietQuyenList = settingAccess.GetPermissionsByRoleId(roleId);
            return chiTietQuyenList.Any(p => p.ChucNang == chucNang && p.HanhDong == hanhDong);
        }

        */





    }
}
