using System;
using System.Collections.Generic;
using DAL;
using DTO;
using static DTO.NhapThuoc;

namespace BLL
{
    public class NhapThuocBLL
    {
        private readonly NhapThuocAccess _access = new NhapThuocAccess();

        // Lấy danh sách tên thuốc (cho combobox)
        public List<string> GetDanhSachTenThuoc()
        {
            return _access.GetDanhSachTenThuoc();
        }

        // Lấy thuốc theo tên
        public ThuocDTO GetThuocByTen(string tenThuoc)
        {
            return _access.GetThuocByTen(tenThuoc);
        }

        // Thêm thuốc mới, trả ID mới
        public int AddNewThuoc(ThuocDTO thuoc)
        {
            return _access.InsertThuoc(thuoc);
        }

        // Tạo phiếu nhập thuốc, trả ID mới
        public int CreatePhieuNhapThuoc(DateTime ngayNhap)
        {
            return _access.CreatePhieuNhapThuoc(ngayNhap);
        }

        // Thêm chi tiết phiếu nhập
        public void AddChiTietPhieuNhap(ChiTietPhieuNhapThuocDTO ct)
        {
            _access.InsertChiTietPhieuNhap(ct);
        }

        // Lưu danh sách thuốc trong một phiếu
        public void NhapDanhSachThuoc(List<ChiTietPhieuNhapThuocDTO> danhSach, DateTime ngayNhap)
        {
            int idPhieu = CreatePhieuNhapThuoc(ngayNhap);
            foreach (var ct in danhSach)
            {
                ct.ID_PhieuNhapThuoc = idPhieu;
                AddChiTietPhieuNhap(ct);
            }
        }

        // Lấy danh sách phiếu nhập thuốc
        public List<PhieuNhapThuocDTO> GetDanhSachPhieuNhap()
        {
            return _access.GetAllPhieuNhap();
        }

        // Cập nhật thông tin thuốc và tăng số lượng tồn
        public void UpdateThuocAndIncreaseQuantity(ThuocDTO thuoc, int soLuongThem)
        {
            _access.UpdateThuocAndIncreaseQuantity(thuoc, soLuongThem);
        }

        // Lấy danh sách đơn vị tính
        public List<DonViTinhDTO> GetAllDVT()
        {
            return _access.GetAllDVT();
        }

        // Lấy danh sách cách dùng
        public List<CachDungDTO> GetAllCachDung()
        {
            return _access.GetAllCachDung();
        }

        // Lấy danh sách chi tiết phiếu nhập theo ID
        public List<ChiTietPhieuNhapThuocDTO> GetChiTietPhieuNhap(int idPhieu)
        {
            return _access.GetChiTietPhieuNhap(idPhieu);
        }
    }
}