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
using ClinicManagement.Utils;

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

                imgThuoc.Source = ImageHelper.LoadImage(selectedDrug.HinhAnh);
            }

            currentDrug = selectedDrug; // Store for saving changes
        }

        private void EditDrug_Click(object sender, RoutedEventArgs e)
        {
            currentDrug.TenThuoc = TenThuoccomboBox.Text;
            currentDrug.TenDVT = DvtcomboBox.Text;
            currentDrug.CachDung = CachDungcomboBox.Text;
            currentDrug.SoLuongTon = int.Parse(textBoxSoLuongNhap.Text);
            currentDrug.ThanhPhan = ThanhPhancomboBox.Text;
            currentDrug.XuatXu = XuatXucomboBox.Text;
            currentDrug.DonGiaNhap = double.Parse(textBoxDonGiaNhap.Text);
            currentDrug.TyLeGiaBan = decimal.Parse(textBoxTyLeGiaBan.Text);

            if (!string.IsNullOrEmpty(imagePath))
                currentDrug.HinhAnh = PathHelper.GetRelativePath(imagePath);

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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
