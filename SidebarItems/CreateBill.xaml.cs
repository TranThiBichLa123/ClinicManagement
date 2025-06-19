using BLL;
using DTO;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Data.SqlClient;

namespace ClinicManagement.SidebarItems
{
    public partial class CreateBill : UserControl
    {
        private BillService service = new BillService();
        private readonly PhanQuyenBLL phanQuyenBLL = new PhanQuyenBLL();
        private readonly LoginLogBLL loginLogBLL = new LoginLogBLL();

        public string Account { get; private set; }

        public CreateBill() { }

        public CreateBill(string userEmail)
        {
            InitializeComponent();
            Account = userEmail;

            int nhomQuyen = phanQuyenBLL.LayNhomTheoEmail(Account);
            var danhSachQuyen = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(nhomQuyen);

            PhanQuyenHelper.DanhSachQuyen = danhSachQuyen;
            UserSession.Email = Account;
            UserSession.NhomQuyen = nhomQuyen;
            UserSession.DanhSachChucNang = danhSachQuyen;
        }

        private bool HasPermission(int chucNangId)
        {
            return UserSession.DanhSachChucNang.Contains(chucNangId);
        }

        private bool DenyIfNoPermission(int chucNangId)
        {
            if (!HasPermission(chucNangId))
            {
                MessageBox.Show("Bạn không có quyền thực hiện chức năng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var danhSachThuNgan = service.GetNhanVienThuNgan();
            cbNhanVien.ItemsSource = danhSachThuNgan;
            cbNhanVien.DisplayMemberPath = "HoTenNV";
            cbNhanVien.SelectedValuePath = "ID_NhanVien";
        }

        private void BtnLapHoaDon_Click(object sender, RoutedEventArgs e)
        {
            if (DenyIfNoPermission(17)) return;

            if (string.IsNullOrWhiteSpace(txtMaPhieuKham.Text) ||
                cbNhanVien.SelectedValue == null ||
                dpNgayLap.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int idPhieuKham = int.Parse(txtMaPhieuKham.Text);
                int idNhanVien = Convert.ToInt32(cbNhanVien.SelectedValue);
                DateTime ngayLap = dpNgayLap.SelectedDate.Value;

                var existingHoaDon = service.GetHoaDon(idPhieuKham);
                if (existingHoaDon != null)
                {
                    MessageBox.Show("Phiếu khám này đã được lập hóa đơn trước đó!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int idHoaDon = service.TaoHoaDon(idPhieuKham, idNhanVien, ngayLap);

                service.CapNhatBaoCaoSauKhiTaoHoaDon(ngayLap.Month, ngayLap.Year);

                FillThongTinHoaDon(idPhieuKham);

                MessageBox.Show("Lập hóa đơn thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Mã phiếu khám hoặc mã nhân viên không hợp lệ!", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("FOREIGN KEY") && ex.Message.Contains("FK_HOADON_ID_Phieu"))
                {
                    MessageBox.Show("Phiếu khám không tồn tại!", "Lỗi lập hóa đơn", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Đã xảy ra lỗi khi lập hóa đơn. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Phiếu khám không tồn tại!", "Lỗi lập hóa đơn", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnXemHoaDon_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaPhieuKham.Text))
            {
                MessageBox.Show("Vui lòng nhập mã phiếu khám!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int idPhieuKham = int.Parse(txtMaPhieuKham.Text);
                FillThongTinHoaDon(idPhieuKham);
            }
            catch (Exception)
            {
                MessageBox.Show("Không thể lấy thông tin hóa đơn. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FillThongTinHoaDon(int idPhieuKham)
        {
            var thongTin = service.GetHoaDon(idPhieuKham);
            var chiTiet = service.GetChiTietHoaDon(idPhieuKham);

            if (thongTin == null)
            {
                MessageBox.Show("Không tìm thấy hóa đơn cho phiếu khám này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            txtMaHoaDon.Text = thongTin.MaHoaDon.ToString();
            txtNgayLap.Text = thongTin.NgayLap.ToString("dd/MM/yyyy");
            txtBenhNhan.Text = thongTin.TenBenhNhan;
            txtTongTien.Text = thongTin.TongTien.ToString("N0");

            cbNhanVien.SelectedValue = thongTin.MaNhanVien;
            dgChiTiet.ItemsSource = chiTiet;
            dpNgayLap.SelectedDate = thongTin.NgayLap;
        }

        private void BtnCapNhatHoaDon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaHoaDon.Text))
                {
                    MessageBox.Show("Vui lòng xem hóa đơn trước khi cập nhật!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int idPhieuKham = int.Parse(txtMaPhieuKham.Text);
                int idNhanVien = Convert.ToInt32(cbNhanVien.SelectedValue);
                DateTime ngayLap = dpNgayLap.SelectedDate ?? DateTime.Now;

                bool success = service.CapNhatHoaDon(idPhieuKham, idNhanVien, ngayLap);

                if (success)
                {
                    MessageBox.Show("Cập nhật hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    FillThongTinHoaDon(idPhieuKham);
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnXoaHoaDon_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa hóa đơn này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    int idPhieuKham = int.Parse(txtMaPhieuKham.Text);
                    bool success = service.XoaHoaDon(idPhieuKham);

                    if (success)
                    {
                        MessageBox.Show("Xóa hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtMaHoaDon.Text = "";
                        txtNgayLap.Text = "";
                        txtBenhNhan.Text = "";
                        txtTongTien.Text = "";
                        dgChiTiet.ItemsSource = null;
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Đã xảy ra lỗi khi xóa hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void dgChiTiet_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgChiTiet.SelectedItem is ChiTietHoaDon selected && selected.MoTa == "Tiền thuốc")
            {
                var items = dgChiTiet.ItemsSource as List<ChiTietHoaDon>;
                if (items == null) return;

                bool anyVisible = items.Any(x => x.IsDrugDetail && x.IsVisible);
                foreach (var item in items)
                {
                    if (item.IsDrugDetail)
                        item.IsVisible = !anyVisible;
                }

                dgChiTiet.Items.Refresh();
            }
        }
    }
}
