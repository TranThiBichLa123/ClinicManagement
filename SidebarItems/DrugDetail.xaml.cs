
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
using ClinicManagement.Utils;
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
                imgThuoc.Source = ImageHelper.LoadImage(selectedDrug.HinhAnh);
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
    }
}
