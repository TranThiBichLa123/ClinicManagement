using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using DAL;
namespace BLL
{
    public class NewDrugBLL
    {
        private readonly NewDrugAccess newDrugAccess = new NewDrugAccess(); 

        private readonly NewDrugAccess drugAccess = new NewDrugAccess();
    


        public List<string> LayTenThuoc()
        {
            return drugAccess.GetDanhSachTenThuoc();
        }
        public List<string> LayDsNgayNhap()
        {
            return drugAccess.GetDsNgayNhap();
        }

        public List<string> LayThanhPhan()
        {
            return drugAccess.GetDanhSachThanhPhan();
        }

        public List<string> LayXuatXu()
        {
            return drugAccess.GetDanhSachXuatXu();
        }

        public bool AddNewDrug(NewDrug newDrug)
        {
            ValidateDrug(newDrug);
            return newDrugAccess.InsertDanhSachThuoc(new List<NewDrug> { newDrug });
        }

        private void ValidateDrug(NewDrug drug)
        {
            if (string.IsNullOrWhiteSpace(drug.TenThuoc))
                throw new Exception("Tên thuốc không được để trống.");

            if (string.IsNullOrWhiteSpace(drug.TenDVT))
                throw new Exception("Vui lòng chọn đơn vị tính.");

            if (string.IsNullOrWhiteSpace(drug.MoTaCachDung))
                throw new Exception("Vui lòng chọn cách dùng.");

            if (string.IsNullOrWhiteSpace(drug.ThanhPhan))
                throw new Exception("Vui lòng nhập thành phần.");

            if (string.IsNullOrWhiteSpace(drug.XuatXu))
                throw new Exception("Vui lòng nhập xuất xứ.");

            if (drug.DonGiaNhap <= 0)
                throw new Exception("Đơn giá nhập phải lớn hơn 0.");

            if (drug.TyLeGiaBan <= 0)
                throw new Exception("Tỷ lệ giá bán phải lớn hơn 0.");

            if (drug.HanSuDung == null || drug.HanSuDung <= DateTime.Today)
                throw new Exception("Hạn sử dụng phải lớn hơn ngày hiện tại.");

            if (string.IsNullOrWhiteSpace(drug.HinhAnh))
                throw new Exception("Vui lòng chọn ảnh thuốc.");
        }

        //nhập danh sách thuốc dùng cho datagrid
        public bool AddDanhSachThuoc(List<NewDrug> danhSachThuoc)
        {
            foreach (var drug in danhSachThuoc)
            {
                ValidateDrug(drug); // kiểm tra từng thuốc
            }

            return newDrugAccess.InsertDanhSachThuoc(danhSachThuoc); // đúng
        }
    }
}
