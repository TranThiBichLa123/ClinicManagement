using System;
using System.Collections.Generic;
using DAL;
using DTO;

namespace BLL
{
    public class BillService
    {
        private BillAccess dal = new BillAccess();

        public int TaoHoaDon(int idPhieuKham, int idNhanVien, DateTime ngay)
        {
            return dal.InsertHoaDon(idPhieuKham, idNhanVien, ngay);
        }

        public HoaDon GetHoaDon(int idPhieuKham)
        {
            return dal.GetThongTinHoaDon(idPhieuKham);
        }

        public List<ChiTietHoaDon> GetChiTietHoaDon(int idPhieuKham)
        {
            return dal.GetChiTietHoaDon(idPhieuKham);
        }
        public List<NhanVien_ThuNgan> GetNhanVienThuNgan()
        {
            return dal.GetThuNganDangLam();
        }
        public bool CapNhatHoaDon(int idPhieuKham, int idNhanVien, DateTime ngay)
        {
            return dal.CapNhatHoaDon(idPhieuKham, idNhanVien, ngay);
        }

        public bool XoaHoaDon(int idPhieuKham)
        {
            return dal.XoaHoaDon(idPhieuKham);
        }
        public List<HoaDon> GetDanhSachHoaDon()
        {
            return dal.GetDanhSachHoaDon();
        }

        public List<HoaDon> GetDanhSachHoaDonTheoNhanVien(int idNhanVien)
        {
            return dal.GetDanhSachHoaDonTheoNhanVien(idNhanVien);
        }
    }

}
