using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class PhanQuyenBLL
    {
        private PhanQuyenDAL dal = new PhanQuyenDAL();

        public void LuuPhanQuyen(int idNhom, List<int> dsIdChucNang)
        {
            dal.LuuPhanQuyen(idNhom, dsIdChucNang);
        }
        public List<int> LayDanhSachIdChucNangTheoNhom(int idNhom)
        {
            return dal.LayDanhSachIdChucNangTheoNhom(idNhom);
        }
        public int LayNhomTheoEmail(string email)
        {
            return (int)dal.LayNhomTheoEmail(email);
        }
        public int LayIDNhanVienTheoEmail(string email)
        {
            return (int)dal.LayIDNhanVienTheoEmail(email);
        }
        public int ThemNhomMoiVaPhanQuyen(string tenNhom, List<int> dsChucNang)
        {
            return dal.ThemNhomMoiVaPhanQuyen(tenNhom, dsChucNang);
        }
        //public void GhiLogTruyCap(string email, string moTa)
        //{
        //    dal.GhiLog(email, moTa);
        //}
    }
}
