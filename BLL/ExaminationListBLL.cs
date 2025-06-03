using System;
using System.Data;
using DAL;

namespace BLL
{
    public class ExaminationListBLL
    {
        private ExaminationListDAL dal = new ExaminationListDAL();

        public DataTable GetDanhSachTiepNhan(DateTime ngayKham)
        {
            return dal.GetDanhSachTiepNhan(ngayKham);
        }

        public int GetSLBNMax()
        {
            return dal.GetSLBNMax();
        }

        public int GetSLBNTrongNgay(DateTime ngayKham)
        {
            return dal.GetSLBNTrongNgay(ngayKham);
        }

        public bool CheckDaCoPK(int idTiepNhan)
        {
            return dal.CheckDaCoPK(idTiepNhan);
        }

        public int InsertTiepNhan(string idBenhNhan, string idNhanVien, DateTime ngayTiepNhan, string caTiepNhan)
        {
            return dal.InsertTiepNhan(idBenhNhan, idNhanVien, ngayTiepNhan, caTiepNhan);
        }

        public int UpdateTiepNhan(string idBenhNhan, string idNhanVien, DateTime ngayTiepNhan, string caTiepNhan,
                                   DateTime oldNgayTiepNhan, string oldCaTiepNhan, string oldIdBenhNhan, string oldIdNhanVien)
        {
            return dal.UpdateTiepNhan(idBenhNhan, idNhanVien, ngayTiepNhan, caTiepNhan,
                                       oldNgayTiepNhan, oldCaTiepNhan, oldIdBenhNhan, oldIdNhanVien);
        }

        public int DeleteTiepNhan(string idBenhNhan, DateTime ngayTiepNhan, string caTiepNhan, string idNhanVien)
        {
            return dal.DeleteTiepNhan(idBenhNhan, ngayTiepNhan, caTiepNhan, idNhanVien);
        }
    }
}
