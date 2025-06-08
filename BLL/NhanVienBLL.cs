using DAL;
using DAL.DAL;
using System;

namespace BLL
{
    public class NhanVienBLL
    {
        private readonly NhanVienDAL dal = new NhanVienDAL();

        public string GetTrangThaiNhanVien(string email)
        {
            return dal.GetTrangThaiNhanVien(email);
        }

        public void KhoaTaiKhoan(string email)
        {
            dal.UpdateTrangThai(email, "Bị khóa");
        }

        public void MoKhoaTaiKhoan(string email)
        {
            dal.UpdateTrangThai(email, "Đang làm việc");
        }

        public bool TonTaiEmail(string email)
        {
            return dal.CheckEmailExists(email);
        }

        public int? LayIDNhanVienTheoEmail(string email)
        {
            return dal.GetIDNhanVienByEmail(email);
        }
        public void CapNhatTrangThai(string email, string trangThai)
        {
            dal.CapNhatTrangThai(email, trangThai);
        }
    }
}
