using System.Collections.Generic;
using DAL;
using DTO;

namespace BLL
{
    public class RoleManagementBLL
    {
        private readonly RoleManagementAccess roleAccess = new RoleManagementAccess();

        // Lấy danh sách tất cả nhóm người dùng
        public List<RoleManagement> GetAllRoles()
        {
            return roleAccess.GetAllRoles();
        }

        // Thêm nhóm quyền mới
        public bool AddRole(string tenVaiTro)
        {
            return roleAccess.InsertRole(tenVaiTro);
        }

        // Cập nhật nhóm quyền
        public bool UpdateRole(int idVaiTro, string tenVaiTro)
        {
            return roleAccess.UpdateRole(idVaiTro, tenVaiTro);
        }

        // Xoá nhóm quyền
        public bool DeleteRole(int idVaiTro)
        {
            return roleAccess.DeleteRole(idVaiTro);
        }

        public List<AccountViewModel> GetAllAccounts()
        {
            return roleAccess.GetAllAccounts();
        }
        public bool XoaNhomQuyen(int idNhom)
        {
            return roleAccess.XoaNhomQuyen(idNhom);
        }
        public bool CoTaiKhoanDangDungNhom(int idNhom)
        {
            return roleAccess.CoTaiKhoanDangDungNhom(idNhom);
        }
    }
}
