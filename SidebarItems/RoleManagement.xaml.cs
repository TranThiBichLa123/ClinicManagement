using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BLL;
using DTO;

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

        private void btnAddRole_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var addWindow = new EditRole();
            if (addWindow.ShowDialog() == true)
            {
                LoadRoles();
            }
        }
        private void accountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            accountDataGrid.SelectedIndex = -1; // Bỏ chọn ngay lập tức
        }


        private void EditRoleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button?.Tag is int idVaiTro)
            {
                var editWindow = new EditRole();
                var role = roleBLL.GetAllRoles().Find(r => r.ID_VaiTro == idVaiTro);
                if (role != null)
                {
                    editWindow.RoleCodeTextBox.Text = role.ID_VaiTro.ToString();
                    editWindow.RoleNameTextBox.Text = role.TenVaiTro;
                }

                if (editWindow.ShowDialog() == true)
                {
                    LoadRoles();
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
