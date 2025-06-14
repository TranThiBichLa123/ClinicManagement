using BLL;
using DTO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ClinicManagement.SidebarItems
{
 
    public partial class EditDrug : Window
    {
        private string imagePath;
        private DTO.Drug currentDrug;
        public EditDrug(DTO.Drug selectedDrug)
        {
            InitializeComponent();

            CachDungcomboBox.Text = selectedDrug.CachDung;
            DvtcomboBox.Text = selectedDrug.TenDVT;
            TenThuoccomboBox.Text = selectedDrug.TenThuoc;
            ThanhPhancomboBox.Text = selectedDrug.ThanhPhan;
            textBoxDonGiaNhap.Text = selectedDrug.DonGiaNhap.ToString();
            XuatXucomboBox.Text = selectedDrug.XuatXu;
            textBoxSoLuongNhap.Text = selectedDrug.SoLuongTon.ToString();
            textBoxTyLeGiaBan.Text = selectedDrug.TyLeGiaBan.ToString();

            if (!string.IsNullOrEmpty(selectedDrug.HinhAnh))
            {

                LoadDrugImage(selectedDrug.HinhAnh); //  Gọi hàm riêng xử lý
            }

            currentDrug = selectedDrug; // Store for saving changes
        }

        private void EditDrug_Click(object sender, RoutedEventArgs e)
        {
            currentDrug.TenThuoc = 
            currentDrug.TenThuoc = TenThuoccomboBox.Text;
            currentDrug.TenDVT = DvtcomboBox.Text;
            currentDrug.CachDung = CachDungcomboBox.Text;

            currentDrug.ThanhPhan = ThanhPhancomboBox.Text;
            currentDrug.XuatXu = XuatXucomboBox.Text;
            currentDrug.DonGiaNhap = double.Parse(textBoxDonGiaNhap.Text);
            currentDrug.TyLeGiaBan = decimal.Parse(textBoxTyLeGiaBan.Text);
            currentDrug.HinhAnh = imagePath ?? currentDrug.HinhAnh;

            bool result = new DrugBLL().UpdateDrug(currentDrug); 

    if (result)
    {
        MessageBox.Show("Cập nhật thành công!");
        this.Close();
    }
    else
    {
        MessageBox.Show("Cập nhật thất bại!");
    }

        }
        private void LoadDrugImage(string relativePath)
        {
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            if (File.Exists(fullPath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgThuoc.Source = bitmap;
            }
        }

        private void NewDrugImg_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Lấy tên file từ đường dẫn chọn
                    string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);

                    // Đường dẫn tương đối để lưu vào DB
                    string relativePath = System.IO.Path.Combine("img", "THUOC", fileName);

                    // Đường dẫn tuyệt đối đến thư mục đích trong project (bin/Debug/ hoặc bin/Release/)
                    string fullDestination = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                    // Tạo thư mục nếu chưa có
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullDestination));

                    // Sao chép file ảnh đã chọn vào thư mục của project
                    File.Copy(openFileDialog.FileName, fullDestination, true); // true = ghi đè nếu trùng tên

                    // Hiển thị ảnh trong Image control
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullDestination, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    imgThuoc.Source = bitmap;

                    // Gán lại đường dẫn tương đối để lưu vào DB
                    imagePath = relativePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chọn ảnh: " + ex.Message);
                }
            }

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
