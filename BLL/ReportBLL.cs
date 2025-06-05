using System;
using System.Collections.Generic;
using DAL;
using DTO;

namespace BLL
{
    public class ReportBLL
    {
        private ReportAccess dal = new ReportAccess();

        // Trả về tổng doanh thu theo tháng, năm  
        public BaoCaoDoanhThu GetTongDoanhThu(int thang, int nam)
        {
            return dal.GetTongDoanhThu(thang, nam);
        }

        // Trả về chi tiết doanh thu trong tháng (theo từng ngày)  
        public List<CT_BaoCaoDoanhThu> GetChiTietDoanhThu(int thang, int nam)
        {
            return dal.GetChiTietDoanhThu(thang, nam);
        }

        public List<BaoCaoSuDungThuoc> GetBaoCaoSuDungThuoc(int thang, int nam)
        {
            return dal.GetBaoCaoSuDungThuoc(thang, nam);
        }

        public List<ThongKeBenhNhan> GetThongKeBenhNhanTheoNgay()
        {
            // Fix: Use the instance field `dal` to call the non-static method `GetThongKeBenhNhanTheoNgay`.  
            return dal.GetThongKeBenhNhanTheoNgay();
        }

        public List<TopThuoc> GetTopThuocSuDungNhieuNhat()
        {
            return dal.GetTopThuocSuDungNhieuNhat();
        }
        public List<GioiTinhThongKe> GetThongKeGioiTinh()
        {
            return dal.GetThongKeGioiTinh();
        }
        public List<DoTuoiThongKe> GetThongKeDoTuoi()
        {
            return dal.GetThongKeDoTuoi();
        }
        public List<TinhThanhThongKe> GetThongKeTinhThanh()
        {
            return dal.GetThongKeTinhThanh();
        }

        public List<DoanhThuTheoThang> GetDoanhThuTheoThang()
        {
            return dal.GetDoanhThuTheoThang();
        }
        public int GetSoBenhNhanHomNay()
        {
            return dal.GetSoBenhNhanHomNay();
        }
        public List<TienNhapTheoThang> GetTongTienNhapTheoThang()
        {
            return dal.GetTongTienNhapTheoThang();
        }
    }
}
