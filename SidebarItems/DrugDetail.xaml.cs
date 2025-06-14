
using System.Windows;
using DTO;
using BLL;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Controls;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
namespace ClinicManagement.SidebarItems
{

    public partial class DrugDetail : Window
    {
       
        private readonly NewDrugBLL newdrugBLL = new NewDrugBLL();
        private DTO.Drug currentDrug;
        private readonly NewDrugBLL newDrugBLL;

        public List<string> DanhSachNgayNhap { get; set; }

        public DrugDetail(DTO.Drug selectedDrug)
        {

            InitializeComponent();
            this.DataContext = this;
            image_show(null, null); // hoặc bạn gán chính xác event

            newDrugBLL = new NewDrugBLL();
            DanhSachNgayNhap = newDrugBLL.LayDsNgayNhap(); 

           

            CachDungcomboBox.Text = selectedDrug.CachDung;
            DvtcomboBox.Text = selectedDrug.TenDVT;
            TenThuoccomboBox.Text = selectedDrug.TenThuoc;
            ThanhPhancomboBox.Text = selectedDrug.ThanhPhan;
            textBoxDonGiaNhap.Text = selectedDrug.DonGiaNhap.ToString();
            XuatXucomboBox.Text = selectedDrug.XuatXu;
            textBoxSoLuongTon.Text = selectedDrug.SoLuongTon.ToString();
            textBoxTyLeGiaBan.Text = selectedDrug.TyLeGiaBan.ToString();

            if (!string.IsNullOrEmpty(selectedDrug.HinhAnh))
            {
                imgThuoc.Source = new BitmapImage(new Uri(selectedDrug.HinhAnh, UriKind.RelativeOrAbsolute));
            }

            currentDrug = selectedDrug;
        }   

        private void NgayNhapLoThuocCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            // Vì ItemsSource là List<string>, nên lấy trực tiếp
            string selectedDrugDate = e.AddedItems[0]?.ToString();
            if (string.IsNullOrEmpty(selectedDrugDate))
                return;

            // Gọi BLL để lấy thuốc theo ngày nhập
            var existingDrug = new DrugBLL().GetHsdByNgayNhap(selectedDrugDate);

            if (existingDrug != null)
            {
                // Gán lại hạn sử dụng cho DatePicker
                datePickerHanSuDung.SelectedDate = existingDrug.HanSuDung;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }

        private void image_show(object sender, RoutedEventArgs e)
        {

            if (currentDrug == null || string.IsNullOrEmpty(currentDrug.HinhAnh))
                return;

            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentDrug.HinhAnh);

            if (File.Exists(fullPath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgThuoc.Source = bitmap;
            }
            else
            {
                MessageBox.Show("Không tìm thấy hình ảnh thuốc tại:\n" + fullPath, "Lỗi hình ảnh", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
