using BLL;
using DTO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ClinicManagement.SidebarItems
{
    public partial class NewDrug : Window
    {
        private readonly NewDrugBLL newdrugBLL = new NewDrugBLL(); 

        public List<string> DanhSachTenThuoc { get; set; }
        public List<string> DanhSachThanhPhan { get; set; }
        public List<string> DanhSachXuatXu { get; set; }
        private string imagePath;
        private List<DTO.NewDrug> danhSachThuoc = new List<DTO.NewDrug>();


        public NewDrug()
        {
            InitializeComponent();
            newDrugBLL = new NewDrugBLL(); 
            DanhSachTenThuoc = newdrugBLL.LayTenThuoc();
            DanhSachThanhPhan = newdrugBLL.LayThanhPhan();
            DanhSachXuatXu = newdrugBLL.LayXuatXu();

            this.DataContext = this;
        }

        private void NewDrugImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgThuoc.Source = bitmap;

                 imagePath = openFileDialog.FileName;
            }

        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(TenThuoccomboBox.Text) || string.IsNullOrWhiteSpace(ThanhPhancomboBox.Text) || string.IsNullOrWhiteSpace(CachDungcomboBox.Text) || string.IsNullOrWhiteSpace(textBoxDonGiaNhap.Text) || string.IsNullOrWhiteSpace(DvtcomboBox.Text) || string.IsNullOrWhiteSpace(textBoxTyLeGiaBan.Text) || string.IsNullOrWhiteSpace(textBoxSoLuongNhap.Text) || string.IsNullOrWhiteSpace(XuatXucomboBox.Text) || datePickerHanSuDung.SelectedDate == null)
            {
                System.Windows.MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        private readonly NewDrugBLL newDrugBLL;
        private string avatarImageBytes;
        private void ResetForm()
        {
            TenThuoccomboBox.SelectedIndex = -1;
            DvtcomboBox.SelectedIndex = -1;
            CachDungcomboBox.SelectedIndex = -1;
            ThanhPhancomboBox.SelectedIndex = -1;
            XuatXucomboBox.SelectedIndex = -1;

            textBoxSoLuongNhap.Clear();
            textBoxDonGiaNhap.Clear();
            textBoxTyLeGiaBan.Clear();
            datePickerHanSuDung.SelectedDate = null;

            imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "drugDefault.jpg");

            imgThuoc.Source = new BitmapImage(new Uri(imagePath));
        }

        private void addDrug_Click(object sender, RoutedEventArgs e)
       {
            try
            {
                if (danhSachThuoc.Count == 0)
                {
                    MessageBox.Show("Danh sách thuốc đang trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool isSuccess = newDrugBLL.AddDanhSachThuoc(danhSachThuoc);
                if (isSuccess)
                {
                    MessageBox.Show("Nhập thuốc thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    danhSachThuoc.Clear();
                    drugDataGrid.ItemsSource = null;
                }
                else
                {
                    MessageBox.Show("Nhập thuốc thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

        private void XoaThuoc_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedDrug = button.DataContext as DTO.NewDrug;

            if (selectedDrug != null)
            {
                danhSachThuoc.Remove(selectedDrug);
                drugDataGrid.ItemsSource = null;
                drugDataGrid.ItemsSource = danhSachThuoc;
            }
        }

        private void TenThuoccomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count == 0)
                return;

            string selectedDrugName = (e.AddedItems[0] as ComboBoxItem)?.Content?.ToString()
                                      ?? e.AddedItems[0]?.ToString();

            if (string.IsNullOrEmpty(selectedDrugName))
                return;

            var existingDrug = new DrugBLL().GetDrugByTen(selectedDrugName);

            if (existingDrug != null)
            {
                DvtcomboBox.Text = existingDrug.TenDVT;
                CachDungcomboBox.Text = existingDrug.CachDung;
                ThanhPhancomboBox.Text = existingDrug.ThanhPhan;
                XuatXucomboBox.Text = existingDrug.XuatXu;
                textBoxDonGiaNhap.Text = existingDrug.DonGiaNhap.ToString();
                textBoxTyLeGiaBan.Text = existingDrug.TyLeGiaBan.ToString();

                string fullImagePath = existingDrug.HinhAnh;

                if (!string.IsNullOrEmpty(fullImagePath) && File.Exists(fullImagePath))
                {
                    imagePath = fullImagePath;
                    imgThuoc.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                }
                else
                {
                    imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "drugDefault.jpg");
                    imgThuoc.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                }
            }
        }

        private void ChonThuoc_Click(object sender, RoutedEventArgs e)
        {
            
    if (!ValidateInputs()) return;

    string tenThuoc = TenThuoccomboBox.Text.Trim();
    var existingDrug = danhSachThuoc.Find(d => d.TenThuoc.Equals(tenThuoc, StringComparison.OrdinalIgnoreCase));

    if (existingDrug != null)
    {
        existingDrug.SoLuongNhap += int.Parse(textBoxSoLuongNhap.Text);
    }
    else
    {
        DTO.NewDrug newDrug = new DTO.NewDrug
        {
            TenThuoc = tenThuoc,
            TenDVT = DvtcomboBox.Text,
            MoTaCachDung = CachDungcomboBox.Text,
            ThanhPhan = ThanhPhancomboBox.Text,
            XuatXu = XuatXucomboBox.Text,
            SoLuongNhap = int.Parse(textBoxSoLuongNhap.Text),
            DonGiaNhap = double.Parse(textBoxDonGiaNhap.Text),
            TyLeGiaBan = decimal.Parse(textBoxTyLeGiaBan.Text),
            HanSuDung = (DateTime)datePickerHanSuDung.SelectedDate,
            HinhAnh = imagePath
        };

        danhSachThuoc.Add(newDrug);
    }

    drugDataGrid.ItemsSource = null;
    drugDataGrid.ItemsSource = danhSachThuoc;
    ResetForm();

        }
    }
}
