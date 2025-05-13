using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DTO;
namespace BLL
{
    public class DrugBLL
    {
        private DrugAccess drugAccess = new DrugAccess(); 

        public List<DTO.Drug> GetDrugList()
        {
            return drugAccess.GetDrugList(); 
        }
        public Task<List<DTO.Drug>> SearchDrug1(string keyword)
        {
            return drugAccess.SearchAsync1(keyword);
        }
        public Task<List<DTO.Drug>> SearchDrug2(string keyword)
        {
            return drugAccess.SearchAsync2(keyword);
        }
        public Task<List<DTO.Drug>> SearchDrug3(string keyword)
        {
            return drugAccess.SearchAsync3(keyword);
        }
        public bool DeleteDrug(int idThuoc)
        {
            return new DrugAccess().DeleteDrug(idThuoc); // gọi xuống DAL
        }

        public Drug GetDrugByTen(string tenThuoc)
        {
            return drugAccess.GetDrugByTen(tenThuoc);
        }
        public Drug GetHsdByNgayNhap(string ngayNhap)
        {
            return drugAccess.GetHsdByNgayNhap(ngayNhap);
        }
        public bool UpdateDrug(Drug drug)
        {
            return new DrugAccess().UpdateDrug(drug);
        }
        /*
        public List<string> GetAllDonViTinh()
        {
            return new DrugAccess().GetAllDonViTinh();
        }

        public List<string> GetAllCachDung()
        {
            return new DrugAccess().GetAllCachDung();
        }*/

        /* //  Get số lượng đã bán từ bảng BAOCAOSUDUNGTHUOC
            int daBan = new DrugBLL().GetTongSoLuongDaBan(selectedMonth, selectedYear);
            daBanTextBlock.Text = daBan.ToString();

            //  Get số lượng tồn kho từ bảng THUOC
            int tonKho = new DrugBLL().GetTongSoLuongTonKho();
            tonKhoTextBlock.Text = tonKho.ToString();

            //  Get thống kê tuần để hiển thị lên biểu đồ (tùy cấu trúc dữ liệu bạn xử lý)
            var chartData = new DrugBLL().GetThongKeTheoTuan(selectedMonth, selectedYear);
            UpdateChart(chartData);*/

        public int GetTongSoLuongDaBan(int month, int year)
        {
            return drugAccess.GetTongSoLuongDaBan(month, year);
        }

        public int GetTongSoLuongTonKho(int month, int year)
        {
            return drugAccess.GetTongSoLuongTonKho(month, year);
        }

        public List<ThongKeTuanDTO> GetThongKeTheoTuan(int month, int year)
        {
            return drugAccess.GetThongKeTheoTuan(month, year);
        }
    }
}
