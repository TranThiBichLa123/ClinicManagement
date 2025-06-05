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
    public partial class AddStaff : Window
    {
        private readonly StaffAccountBLL staffBLL = new StaffAccountBLL();
        private string selectedImagePath = string.Empty;
        public Staff NewStaff { get; private set; }


        public AddStaff()
        {
            InitializeComponent();
            LoadVaiTro();
        }

        private void LoadVaiTro()
        {
            cbVaiTroAdd.ItemsSource = staffBLL.GetRoleList();
        }

        private void btnSaveAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var matKhauInput = string.IsNullOrWhiteSpace(txtMatKhauAdd.Text)
    ? "123456"
    : txtMatKhauAdd.Text.Trim();

                var staff = new Staff
                {
                    HoTenNV = txtHoTenAdd.Text.Trim(),
                    Email = txtEmailAdd.Text.Trim(),
                    DienThoai = txtDienThoaiAdd.Text.Trim(),
                    CCCD = txtCCCDAdd.Text.Trim(),
                    DiaChi = txtDiaChiAdd.Text.Trim(),
                    NgaySinh = dpNgaySinhAdd.SelectedDate,
                    GioiTinh = ((ComboBoxItem)cbGioiTinhAdd.SelectedItem)?.Content.ToString(),
                    ID_VaiTro = (int?)cbVaiTroAdd.SelectedValue ?? 0,
                    HinhAnh = selectedImagePath,
                    TrangThai = "Đang làm việc",
                    MatKhau = matKhauInput
                };

                staffBLL.InsertStaff(staff);
                NewStaff = staff;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void btnCancelAdd_Click(object sender, RoutedEventArgs e)
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
                avatarBrushAdd.ImageSource = new BitmapImage(new Uri(selectedImagePath));
            }
        }
    }
}
