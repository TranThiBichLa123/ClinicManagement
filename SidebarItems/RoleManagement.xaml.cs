using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BLL;
using DTO;
using static DTO.PhanQuyen;

namespace ClinicManagement.SidebarItems
{
    public partial class RoleManagement : UserControl
    {
        private readonly RoleManagementBLL roleBLL = new RoleManagementBLL();

        public RoleManagement()
        {
            InitializeComponent();
            TabControl_Setting.SelectedIndex = 0;

            LoadRoles();     // Phân quyền
            LoadAccounts();
        }

        // Tab Phân quyền
        private void LoadRoles()
        {
            List<DTO.RoleManagement> roles = roleBLL.GetAllRoles();
            roleDataGrid.ItemsSource = roles;
        }

        private void btnAddRole_Click(object sender, RoutedEventArgs e)
        {
            // Tạo nhóm rỗng để truyền vào màn hình thêm mới
            var emptyNhom = new NhomNguoiDungDTO
            {
                ID_Nhom = 0, // Có thể gán 0 hoặc -1 nếu chưa có ID
                TenNhom = string.Empty
            };

            var addWindow = new EditRole(emptyNhom); // ✅ Đúng constructor
            if (addWindow.ShowDialog() == true)
            {
                LoadRoles();
            }
        }
        private void accountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            accountDataGrid.SelectedIndex = -1; // Bỏ chọn ngay lập tức
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (roleDataGrid.SelectedItem is DTO.RoleManagement selected)
            {
                if (roleBLL.CoTaiKhoanDangDungNhom(selected.ID_VaiTro))
                {
                    MessageBox.Show("Không thể xóa vì vẫn còn nhân viên đang sử dụng nhóm quyền này.\nHãy chuyển vai trò của họ trước!",
                                    "Ràng buộc dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Bạn có chắc muốn xóa nhóm quyền \"{selected.TenVaiTro}\" không?",
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    bool success = roleBLL.XoaNhomQuyen(selected.ID_VaiTro);

                    if (success)
                    {
                        MessageBox.Show("Đã xóa nhóm quyền!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadRoles();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa nhóm quyền này.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhóm quyền cần xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }





        private void EditRoleButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button?.Tag is int idVaiTro)
            {
                var role = roleBLL.GetAllRoles().Find(r => r.ID_VaiTro == idVaiTro);
                if (role != null)
                {
                    // Tạo DTO phù hợp với EditRole constructor
                    var dto = new NhomNguoiDungDTO
                    {
                        ID_Nhom = role.ID_VaiTro,
                        TenNhom = role.TenVaiTro
                    };

                    var editWindow = new EditRole(dto);
                    if (editWindow.ShowDialog() == true)
                    {
                        LoadRoles();
                    }
                }
            }
        }


        // Tab Tài khoản
        private void LoadAccounts()
        {
            List<AccountViewModel> accounts = roleBLL.GetAllAccounts();
            accountDataGrid.ItemsSource = accounts;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            double radius = 20;

            var rootElement = this.Content as FrameworkElement;
            if (rootElement != null)
            {
                var clipRect = new RectangleGeometry(new Rect(0, 0, rootElement.ActualWidth, rootElement.ActualHeight), radius, radius);
                rootElement.Clip = clipRect;
            }
        }
    }
}
