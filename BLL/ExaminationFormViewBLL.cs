using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using System.Data;
using DAL;

namespace BLL
{
    public class ExaminationFormViewBLL
    {
        private ExaminationFormViewDAL dal = new ExaminationFormViewDAL();

        public DataRow GetPhieuKham(int idTiepNhan)
        {
            return dal.GetPhieuKham(idTiepNhan);
        }

        public DataTable GetToaThuocTheoPhieuKham(int idPhieuKham)
        {
            return dal.GetToaThuocTheoPhieuKham(idPhieuKham);
        }

        public void XoaPhieuKham(int idPhieuKham)
        {
            dal.XoaPhieuKham(idPhieuKham);
        }

        public void DeleteToaThuoc(int idPhieuKham)
        {
            dal.DeleteToaThuoc(idPhieuKham);
        }

        public bool CheckDaXuatHD(int idPhieuKham)
        {
            return dal.CheckDaXuatHD(idPhieuKham);
        }
    }
}
