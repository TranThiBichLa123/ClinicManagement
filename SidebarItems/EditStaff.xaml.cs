using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using DTO;
using BLL;

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
                avatarBrush.ImageSource = new BitmapImage(new Uri(selectedImagePath, UriKind.Absolute));
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
            UpdatedStaff.HinhAnh = selectedImagePath;

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
                selectedImagePath = dialog.FileName;
                avatarBrush.ImageSource = new BitmapImage(new Uri(selectedImagePath));
            }
        }
    }
}
