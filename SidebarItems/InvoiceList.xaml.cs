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

        public InvoiceList()
        {
            InitializeComponent();
            Loaded += InvoiceList_Loaded;
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
            DateTime? selectedDate = datePickerFilter.SelectedDate;

            var filtered = originalList.Where(hd =>
            {
                bool matchCategory;

                if (selectedCategory == "Mã hóa đơn")
                    matchCategory = hd.MaHoaDon.ToString().Contains(searchText);
                else if (selectedCategory == "Mã phiếu khám")
                    matchCategory = hd.MaPhieuKham.ToString().Contains(searchText);
                else if (selectedCategory == "Nhân viên")
                    matchCategory = hd.MaNhanVien.ToString().Contains(searchText);
                else
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




    }
}
