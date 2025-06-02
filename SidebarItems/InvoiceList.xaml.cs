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
    /// Interaction logic for InvoiceList.xaml
    /// </summary>
    public partial class InvoiceList : UserControl
    {
        private BillService service = new BillService();

        private Doctor _mainWindow;

        public InvoiceList(Doctor mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }


        private void InvoiceList_Loaded(object sender, RoutedEventArgs e)
        {
            originalList = service.GetDanhSachHoaDon(); // GÁN VÔ ĐÂY
            billDataGrid.ItemsSource = originalList;
        }

        private List<HoaDon> originalList = new List<HoaDon>();

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = textBoxSearch.Text.Trim().ToLower();
            string selectedCategory = (searchCategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            DateTime? selectedDate = datePickerSearch.SelectedDate; // <-- đúng tên biến!

            var filtered = originalList.Where(hd =>
            {
                bool matchCategory;

                if (selectedCategory == "Mã hóa đơn")
                    matchCategory = hd.MaHoaDon.ToString().ToLower().Contains(searchText);
                else if (selectedCategory == "Mã phiếu khám")
                    matchCategory = hd.MaPhieuKham.ToString().ToLower().Contains(searchText);
                else if (selectedCategory == "Mã nhân viên")
                    matchCategory = hd.MaNhanVien.ToString().ToLower().Contains(searchText);
                else // "Tất cả"
                {
                    matchCategory = hd.MaHoaDon.ToString().ToLower().Contains(searchText)
                                 || hd.MaPhieuKham.ToString().ToLower().Contains(searchText)
                                 || hd.MaNhanVien.ToString().ToLower().Contains(searchText);
                }

                bool matchDate = selectedDate == null || hd.NgayLap.Date == selectedDate.Value.Date;

                return matchCategory && matchDate;
            }).ToList();

            billDataGrid.ItemsSource = filtered;
        }


        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            // Giả lập lại gọi hàm giống khi gõ
            textBoxSearch_TextChanged(null, null);
        }

        private void DeleteBill_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa hóa đơn này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Lấy đối tượng hóa đơn từ dòng được click
                var bill = (sender as FrameworkElement)?.DataContext as HoaDon;

                if (bill != null)
                {
                    bool success = service.XoaHoaDon(bill.MaHoaDon); // hoặc MaPhieuKham nếu xóa theo mã phiếu

                    if (success)
                    {
                        MessageBox.Show("Đã xóa hóa đơn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Xóa khỏi danh sách và refresh datagrid
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
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (billDataGrid.SelectedItem is HoaDon selected)
            {
                _mainWindow.LoadUserControl(new EditBill(selected.MaPhieuKham, _mainWindow));
            }
        }




    }
}
