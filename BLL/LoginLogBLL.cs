using System;
using System.Collections.Generic;
using DAL;
using DTO;
using static BLL.LoginLogBLL;

namespace BLL
{
    public class LoginLogBLL
    {
        private readonly LoginLogDAL logDAL = new LoginLogDAL();

        // Lấy toàn bộ log
        public List<LoginLogDTO> GetLogData()
        {
            return logDAL.GetAllLogs();
        }

        // Ghi log thất bại khi đăng nhập sai
        public void GhiLogThatBai(string email)
        {
            logDAL.InsertOrUpdateFailedAttempt(email);
        }
        public void GhiLog(string email, string trangThai, int soLanThatBai, string hanhDong)
        {
            logDAL.InsertLog(email, trangThai, soLanThatBai, hanhDong);
        }

        // Mở khóa tài khoản
        public void MoKhoaTaiKhoan(string email, string trangThai)
        {
            logDAL.MoKhoaTaiKhoan(email, trangThai);
        }

        // Kiểm tra trạng thái tài khoản
        public string GetTrangThai(string email)
        {
            return logDAL.GetTrangThaiByEmail(email);
        }
        public bool KiemTraDangNhap(string email, string password)
        {
            if (logDAL.XacThucTaiKhoan(email, password)) // kiểm tra trong DAL
            {
                logDAL.InsertLog(email, "Đang làm việc", 0, "Đăng nhập"); // reset
                return true;
            }
            else
            {
                logDAL.InsertOrUpdateFailedAttempt(email);

                string trangThai = logDAL.GetTrangThaiByEmail(email);
                if (trangThai == "Bị khóa")
                {
                    throw new Exception("Tài khoản đã bị khóa do đăng nhập sai nhiều lần.");
                }

                return false;
            }
        }

      
    }
}
