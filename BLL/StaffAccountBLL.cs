using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using DAL;
using DTO;

namespace BLL
{
    public class StaffAccountBLL
    {
        private readonly StaffAccountAccess _access = new StaffAccountAccess();

        public List<StaffAccount> GetRoleList() => _access.GetRoleList();

        public List<AccountManager> GetAccountList() => _access.GetAccountList();

        public List<Staff> GetStaffList() => _access.GetStaffList();

        public bool InsertStaff(Staff nv)
        {
            ValidateStaff(nv);
            return _access.InsertStaff(nv);
        }

        public bool UpdateStaff(Staff nv)
        {
            ValidateStaff(nv);
            return _access.UpdateStaff(nv);
        }

        public bool DeleteStaff(int id)
        {
            return _access.DeleteStaff(id);
        }


        public bool DeleteMultipleStaff(List<int> ids)
        {
            return _access.DeleteMultipleStaff(ids);
        }


        private void ValidateStaff(Staff nv)
        {
            if (string.IsNullOrWhiteSpace(nv.HoTenNV))
                throw new Exception("Họ tên không được để trống.");

            if (!nv.NgaySinh.HasValue)
                throw new Exception("Vui lòng chọn ngày sinh.");

            if (nv.NgaySinh >= DateTime.Today)
                throw new Exception("Ngày sinh không hợp lệ.");

            int tuoi = DateTime.Today.Year - nv.NgaySinh.Value.Year;
            if (nv.NgaySinh.Value.Date > DateTime.Today.AddYears(-tuoi)) tuoi--;
            if (tuoi < 18)
                throw new Exception("Nhân viên phải đủ 18 tuổi trở lên.");

            if (!IsValidEmail(nv.Email))
                throw new Exception("Email không hợp lệ.");
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public Staff GetStaffById(int id)
        {
            return _access.GetStaffById(id);
        }

    }
}
