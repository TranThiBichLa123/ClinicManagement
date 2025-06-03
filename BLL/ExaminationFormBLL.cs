using System;
using System.Collections.Generic;
using System.Data;
using DAL;
using DTO;

namespace BLL
{
    public class ExaminationFormBLL
    {
        private ExaminationFormDAL dal = new ExaminationFormDAL();

        public DataTable GetLoaiBenh()
        {
            return dal.GetLoaiBenh();
        }

        public DataTable GetThuocList()
        {
            return dal.GetThuocList();
        }

        public DataRow GetBenhNhanInfo(int idBenhNhan)
        {
            return dal.GetThongTinBenhNhan(idBenhNhan);
        }

        public DataRow GetPhieuKham(int idPK)
        {
            return dal.GetPhieuKham(idPK);
        }

        public DataTable GetToaThuoc(int idPK)
        {
            return dal.GetToaThuocTheoPhieuKham(idPK);
        }

        public DataRow GetChiTietThuoc(string idThuoc)
        {
            return dal.GetChiTietThuoc(idThuoc);
        }

        public int TaoPhieuKham(int idTiepNhan, string caKham, string trieuChung, int idLoaiBenh)
        {
            return dal.InsertPhieuKham(idTiepNhan, caKham, trieuChung, idLoaiBenh);
        }

        public void CapNhatPhieuKham(int idPhieuKham, string caKham, string trieuChung, int idLoaiBenh)
        {
            dal.UpdatePhieuKham(idPhieuKham, caKham, trieuChung, idLoaiBenh);
        }

        public void XoaToaThuoc(int idPhieuKham)
        {
            dal.DeleteToaThuoc(idPhieuKham);
        }

        public void ThemToaThuoc(int idPhieuKham, List<ThuocDaChon> danhSachThuoc)
        {
            foreach (var thuoc in danhSachThuoc)
            {
                dal.InsertToaThuoc(idPhieuKham, thuoc);
            }
        }
    }
}