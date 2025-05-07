using BLL;
using DTO;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;


namespace ClinicManagement.SidebarItems
{
    public partial class CreateBill : UserControl
    {
        private BillService service = new BillService();

        public CreateBill()
        {
            InitializeComponent();
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

                FillThongTinHoaDon(idPhieuKham);
            }
            catch (FormatException)
            {
                MessageBox.Show("Mã phiếu khám hoặc mã nhân viên không hợp lệ!", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lập hóa đơn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        // Xóa dữ liệu trên giao diện
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
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        item.IsVisible = !anyVisible; // toggle
                }

                dgChiTiet.Items.Refresh();
            }
        }


    }
}
