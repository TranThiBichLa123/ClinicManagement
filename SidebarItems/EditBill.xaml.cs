using BLL;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClinicManagement.SidebarItems
{
    /// <summary>
    /// Interaction logic for EditBill.xaml
    /// </summary>
    public partial class EditBill : UserControl
    {
        private int _idPhieuKham;
        private BillService service = new BillService();

        private Doctor _mainWindow;

        public EditBill(int idPhieuKham, Doctor mainWindow)
        {
            InitializeComponent();
            _idPhieuKham = idPhieuKham;
            _mainWindow = mainWindow;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var danhSachThuNgan = service.GetNhanVienThuNgan();
            cbNhanVien.ItemsSource = danhSachThuNgan;
            cbNhanVien.DisplayMemberPath = "HoTenNV";
            cbNhanVien.SelectedValuePath = "ID_NhanVien";

            FillThongTinHoaDon(_idPhieuKham);
        }
        private void FillThongTinHoaDon(int idPhieuKham)
        {
            var thongTin = service.GetHoaDon(idPhieuKham);
            var chiTiet = service.GetChiTietHoaDon(idPhieuKham);

            txtMaHoaDon.Text = thongTin.MaHoaDon.ToString();
            txtNgayLap.Text = thongTin.NgayLap.ToString("dd/MM/yyyy");
            txtBenhNhan.Text = thongTin.TenBenhNhan;
            txtTongTien.Text = thongTin.TongTien.ToString("N0");

            txtMaPhieuKham.Text = idPhieuKham.ToString();
            cbNhanVien.SelectedValue = thongTin.MaNhanVien;
            dpNgayLap.SelectedDate = thongTin.NgayLap;
            dgChiTiet.ItemsSource = chiTiet;
        }
        private void BtnCapNhatHoaDon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int idPhieuKham = int.Parse(txtMaPhieuKham.Text);
                int idNhanVien = Convert.ToInt32(cbNhanVien.SelectedValue);
                DateTime ngayLap = dpNgayLap.SelectedDate ?? DateTime.Now;

                bool success = service.CapNhatHoaDon(idPhieuKham, idNhanVien, ngayLap);
                if (success)
                {
                    MessageBox.Show("Cập nhật hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    _mainWindow.LoadUserControl(new InvoiceList(_mainWindow));

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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.LoadUserControl(new InvoiceList(_mainWindow));

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
