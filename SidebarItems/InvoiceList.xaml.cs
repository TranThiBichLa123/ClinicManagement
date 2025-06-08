using BLL;
using DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClinicManagement.SidebarItems
{
    public partial class InvoiceList : UserControl
    {
        private BillService service = new BillService();
        private readonly PhanQuyenBLL phanQuyenBLL = new PhanQuyenBLL();

        private List<HoaDon> originalList = new List<HoaDon>();
        public string Account { get; private set; }
        private Doctor _mainWindow;
        public InvoiceList() { }
        public InvoiceList(string userEmail, Doctor mainWindow)
        {
            InitializeComponent();

            Account = userEmail;
            _mainWindow = mainWindow;

            // Load quyền
            int nhomQuyen = phanQuyenBLL.LayNhomTheoEmail(Account);
            var danhSachQuyen = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(nhomQuyen);

            PhanQuyenHelper.DanhSachQuyen = danhSachQuyen;
            UserSession.Email = Account;
            UserSession.NhomQuyen = nhomQuyen;
            UserSession.DanhSachChucNang = danhSachQuyen;

            // Gắn sự kiện Loaded
            Loaded += InvoiceList_Loaded;
        }

        private void InvoiceList_Loaded(object sender, RoutedEventArgs e)
        {
            int idNhanVien = new PhanQuyenBLL().LayIDNhanVienTheoEmail(UserSession.Email);
            bool coQuyen24 = UserSession.DanhSachChucNang.Contains(24);

            DataTable dt;

            if (coQuyen24)
            {
                // Lấy toàn bộ danh sách hóa đơn
                originalList = service.GetDanhSachHoaDon();
            }
            else
            {
                // Lấy danh sách hóa đơn do chính nhân viên đang đăng nhập tạo
                originalList = service.GetDanhSachHoaDonTheoNhanVien(idNhanVien);
            }
           
            billDataGrid.ItemsSource = originalList;
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

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = textBoxSearch.Text.Trim().ToLower();
            string selectedCategory = (searchCategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả";

            DateTime? selectedDate = datePickerSearch.SelectedDate;

            var filtered = originalList.Where(hd =>
            {
                bool matchCategory;

                if (selectedCategory == "Mã hóa đơn")
                    matchCategory = hd.MaHoaDon.ToString().Contains(searchText);
                else if (selectedCategory == "Mã phiếu khám")
                    matchCategory = hd.MaPhieuKham.ToString().Contains(searchText);
                else if (selectedCategory == "Mã nhân viên")
                    matchCategory = hd.MaNhanVien.ToString().Contains(searchText);
                else // "Tất cả"
                {
                    matchCategory = hd.MaHoaDon.ToString().Contains(searchText)
                                 || hd.MaPhieuKham.ToString().Contains(searchText)
                                 || hd.MaNhanVien.ToString().Contains(searchText);
                }

                bool matchDate = selectedDate == null || hd.NgayLap.Date == selectedDate.Value.Date;

                return matchCategory && matchDate;
            }).ToList();

            billDataGrid.ItemsSource = filtered;
        }

        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            textBoxSearch_TextChanged(null, null);
        }

        private void DeleteBill_Click(object sender, RoutedEventArgs e)
        {
            if (DenyIfNoPermission(23)) return;

            var bill = (sender as FrameworkElement)?.DataContext as HoaDon;
            if (bill == null) return;

            if (MessageBox.Show("Bạn có chắc muốn xóa hóa đơn này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                bool success = service.XoaHoaDon(bill.MaHoaDon);
                if (success)
                {
                    MessageBox.Show("Đã xóa hóa đơn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    originalList.Remove(bill);
                    billDataGrid.ItemsSource = null;
                    billDataGrid.ItemsSource = originalList;
                }
                else
                {
                    MessageBox.Show("Không thể xóa hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (DenyIfNoPermission(19)) return;

            if (billDataGrid.SelectedItem is HoaDon selected)
            {
                _mainWindow.LoadUserControl(new EditBill(selected.MaPhieuKham, _mainWindow, Account));

            }
        }
    }
}
