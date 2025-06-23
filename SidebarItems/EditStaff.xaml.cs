using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DTO;
using BLL;
using ClinicManagement.Utils;

namespace ClinicManagement.SidebarItems
{
    public partial class EditStaff : Window
    {
        public Staff UpdatedStaff { get; private set; } = new Staff();
        private string selectedImagePath = string.Empty;

        public EditStaff(Staff staffToEdit)
        {
            InitializeComponent();
            LoadVaiTro();
            FillUI(staffToEdit);
        }

        private void LoadVaiTro()
        {
            var roles = new StaffAccountBLL().GetRoleList();
            cbVaiTro.ItemsSource = roles;
        }

        private void FillUI(Staff staff)
        {
            UpdatedStaff.ID_NhanVien = staff.ID_NhanVien;
            txtHoTen.Text = staff.HoTenNV;
            txtEmail.Text = staff.Email;
            txtDienThoai.Text = staff.DienThoai;
            txtCCCD.Text = staff.CCCD;
            txtDiaChi.Text = staff.DiaChi;
            dpNgaySinh.SelectedDate = staff.NgaySinh;

            foreach (ComboBoxItem item in cbGioiTinh.Items)
            {
                if (item.Content.ToString() == staff.GioiTinh)
                {
                    cbGioiTinh.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in cbTrangThai.Items)
            {
                if (item.Content.ToString() == staff.TrangThai)
                {
                    cbTrangThai.SelectedItem = item;
                    break;
                }
            }

            cbVaiTro.SelectedValue = staff.ID_VaiTro;

            if (!string.IsNullOrEmpty(staff.HinhAnh))
            {
                selectedImagePath = staff.HinhAnh;
                string absolutePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, staff.HinhAnh);
                if (System.IO.File.Exists(absolutePath))
                {
                    avatarBrush.ImageSource = new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
                }
                else
                {
                    avatarBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/staffDefault.png")); // fallback nếu cần
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            UpdatedStaff.HoTenNV = txtHoTen.Text.Trim();
            UpdatedStaff.Email = txtEmail.Text.Trim();
            UpdatedStaff.DienThoai = txtDienThoai.Text.Trim();
            UpdatedStaff.CCCD = txtCCCD.Text.Trim();
            UpdatedStaff.DiaChi = txtDiaChi.Text.Trim();
            UpdatedStaff.NgaySinh = dpNgaySinh.SelectedDate;
            UpdatedStaff.GioiTinh = ((ComboBoxItem)cbGioiTinh.SelectedItem)?.Content.ToString();
            UpdatedStaff.TrangThai = ((ComboBoxItem)cbTrangThai.SelectedItem)?.Content.ToString();
            UpdatedStaff.ID_VaiTro = (int?)cbVaiTro.SelectedValue ?? 0;
            UpdatedStaff.HinhAnh = string.IsNullOrEmpty(selectedImagePath)
     ? UpdatedStaff.HinhAnh
     : PathHelper.GetRelativePath(selectedImagePath);

            try
            {
                var bll = new StaffAccountBLL();
                bll.UpdateStaff(UpdatedStaff);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void AvatarEllipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Image Files|*.jpg;*.png;*.jpeg" };
            if (dialog.ShowDialog() == true)
            {
                string sourcePath = dialog.FileName;
                string fileName = System.IO.Path.GetFileName(sourcePath);
                string relativePath = $"img/Nhanvien/{fileName}";
                string destPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                try
                {
                    if (!System.IO.File.Exists(destPath))
                    {
                        System.IO.File.Copy(sourcePath, destPath);
                    }

                    // Gán đường dẫn tương đối vào selectedImagePath để lưu xuống DB
                    selectedImagePath = relativePath;

                    // Gán ảnh cho UI
                    avatarBrush.ImageSource = new BitmapImage(new Uri(destPath, UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chọn ảnh: " + ex.Message);
                }
            }
        }

    }
}
