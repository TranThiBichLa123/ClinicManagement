using DTO;
using DAL;
using System.Data;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
namespace BLL
{
    public class AccountBLL
    {
        AccountAccess accAccess = new AccountAccess();
        public string CheckLogin(Account acc, out string userRole)
        {
            userRole = string.Empty; // Biến để lưu quyền người dùng

            if (string.IsNullOrEmpty(acc.Email))
            {
                return "request_taikhoan";  // Kiểm tra nếu tài khoản trống
            }
            if (string.IsNullOrEmpty(acc.MatKhau))
            {
                return "request_password";  // Kiểm tra nếu mật khẩu trống
            }

            // Gọi phương thức DAL để kiểm tra tài khoản và mật khẩu
            DataTable result = accAccess.GetUserWithRole(acc);
            if (result.Rows.Count > 0)
            {
                userRole = result.Rows[0]["ID_NHOM"].ToString();
                return "success";  // Đăng nhập thành công
            }
            return "invalid_login";  // Nếu không tìm thấy người dùng
        }


        private DatabaseAccess dbAccess = new DatabaseAccess();

        public DataTable GetTaiKhoan()
        {
            string query = "SELECT * FROM NHANVIEN"; // Ví dụ
            return dbAccess.GetData(query);
        }
    }
    public class BenhNhanBLL
    {
        private readonly PatientDAL patientDal = new PatientDAL();
        public DataTable LoadPatientList(string nameKeyword = "", string idKeyword = "", string loaiBenhKeyword = "", string trieuChungKeyword = "", string ngayDK = "")
        {
            return patientDal.LoadPatientList(nameKeyword, idKeyword, loaiBenhKeyword, trieuChungKeyword, ngayDK);
        }
        public bool AddPatientToDatabase(BenhNhan patient)
        {
            return patientDal.AddPatientToDatabase(patient);
        }
        public bool UpdatePatient(BenhNhan patient)
        {
            return patientDal.UpdatePatient(patient);
        }
        public int DeletePatient(int id)
        {
            return patientDal.DeletePatient(id);
        }
    }
    public class ExaminationBLL
    {
        private readonly ExaminationDAL examinationDal = new ExaminationDAL();
        public BenhNhan GetBenhNhanById(string id)
        {
            return examinationDal.GetBenhNhanById(id);
        }
        public List<PhieuKham> LoadPhieuKham(string idBenhNhan)
        {
            return examinationDal.LoadPhieuKham(idBenhNhan);
        }
        public List<Thuoc> LoadDanhSachThuoc(int idPhieuKham)
        {
            return examinationDal.LoadDanhSachThuoc(idPhieuKham);
        }
        public object[] ShowExaminationPopup(int idPhieuKham)
        {
            return examinationDal.ShowExaminationPopup(idPhieuKham);
        }
    }
}
