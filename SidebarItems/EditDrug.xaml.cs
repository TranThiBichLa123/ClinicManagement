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


          /*  // Set Data First
            var cachDungList = new DrugBLL().GetAllCachDung();
            var dvtList = new DrugBLL().GetAllDonViTinh();

            CachDungcomboBox.ItemsSource = cachDungList;
            DvtcomboBox.ItemsSource = dvtList;
          */
            // Assign selected value (safe even if not found)
            CachDungcomboBox.Text = selectedDrug.CachDung;
            DvtcomboBox.Text = selectedDrug.TenDVT;
            TenThuoccomboBox.Text = selectedDrug.TenThuoc;
            ThanhPhancomboBox.Text = selectedDrug.ThanhPhan;
            textBoxDonGiaNhap.Text = selectedDrug.DonGiaNhap.ToString();
            XuatXucomboBox.Text = selectedDrug.XuatXu;
            textBoxSoLuongNhap.Text = selectedDrug.SoLuongTon.ToString();
            textBoxTyLeGiaBan.Text = selectedDrug.TyLeGiaBan.ToString();
           /* datePickerHanSuDung.SelectedDate = selectedDrug.HanSuDung;*/

            if (!string.IsNullOrEmpty(selectedDrug.HinhAnh))
            {
               /* currentDrug.HinhAnh = imagePath ?? selectedDrug.HinhAnh;*/

                imgThuoc.Source = new BitmapImage(new Uri(selectedDrug.HinhAnh, UriKind.Absolute));
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
    }
}
