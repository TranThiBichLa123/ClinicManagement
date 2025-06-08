using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL;
using DTO;

namespace ClinicManagement.SidebarItems
{
    public partial class ReceiptList : UserControl
    {
        private NhapThuocBLL _nhapThuocBLL = new NhapThuocBLL();
        private List<NhapThuoc.PhieuNhapThuocDTO> _allPhieuNhap = new List<NhapThuoc.PhieuNhapThuocDTO>();

        public ReceiptList()
        {
            InitializeComponent();
            Loaded += ReceiptList_Loaded;
        }

        private void ReceiptList_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPhieuNhap();
        }

        /// <summary>
        /// Tải toàn bộ danh sách phiếu nhập
        /// </summary>
        private void LoadPhieuNhap()
        {
            _allPhieuNhap = _nhapThuocBLL.GetDanhSachPhieuNhap();

            if (_allPhieuNhap.Count == 0)
                MessageBox.Show("Không có dữ liệu phiếu nhập", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            receiptDataGrid.ItemsSource = _allPhieuNhap;
        }

        /// <summary>
        /// Tìm kiếm theo ngày nhập được chọn từ DatePicker
        /// </summary>
        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            string input = textBoxSearch.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                receiptDataGrid.ItemsSource = _allPhieuNhap;
                return;
            }

            if (DateTime.TryParseExact(input, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime searchDate))
            {
                var filtered = _allPhieuNhap
                    .Where(p => p.NgayNhap.Date == searchDate)
                    .ToList();

                if (filtered.Count == 0)
                    MessageBox.Show("Không tìm thấy phiếu nhập nào cho ngày này.", "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);

                receiptDataGrid.ItemsSource = filtered;
            }
            else
            {
                MessageBox.Show("Định dạng ngày không hợp lệ. Vui lòng nhập theo định dạng dd/MM/yyyy.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        /// <summary>
        /// Mở form nhập thuốc mới và load lại danh sách
        /// </summary>
        private void AddClick(object sender, RoutedEventArgs e)
        {
            var newDrugWindow = new NewDrug();
            newDrugWindow.ShowDialog();

            LoadPhieuNhap();
        }


        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            // Làm mới danh sách từ database
            _allPhieuNhap = _nhapThuocBLL.GetDanhSachPhieuNhap();

            // Gán lại danh sách vào DataGrid
            receiptDataGrid.ItemsSource = _allPhieuNhap;
            receiptDataGrid.Items.Refresh();

            // Xóa nội dung ô tìm kiếm
            textBoxSearch.Text = "";

            // Thông báo nếu không có dữ liệu
            if (_allPhieuNhap.Count == 0)
            {
                MessageBox.Show("Hiện không có dữ liệu phiếu nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        /// <summary>
        /// Xử lý khi nhấn nút xem chi tiết trong bảng
        /// </summary>
        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is NhapThuoc.PhieuNhapThuocDTO selected)
            {
                var viewWindow = new NewDrug(selected.ID_PhieuNhapThuoc, true);
                viewWindow.ShowDialog();
            }
        }


    }
}
