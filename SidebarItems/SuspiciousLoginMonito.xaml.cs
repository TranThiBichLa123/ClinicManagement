using BLL;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClinicManagement.SidebarItems
{
    /// <summary>
    /// Interaction logic for SuspiciousLoginMonito.xaml
    /// </summary>
    public partial class SuspiciousLoginMonito : UserControl
    {
        public SuspiciousLoginMonito()
        {
            InitializeComponent();
            this.Loaded += SuspiciousLoginMonito_Loaded;
        }
        private void SuspiciousLoginMonito_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshLogs();
        }
        private readonly LoginLogBLL logService = new LoginLogBLL();
        private readonly NhanVienBLL nhanVienBLL = new NhanVienBLL();
        private List<LoginLogDTO> allLogs;
        private void OnFilterClick(object sender, RoutedEventArgs e)
        {
            string email = txtSearchEmail.Text.Trim().ToLower();
            string selectedStatus = (cmbTrangThai.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filtered = allLogs.Where(log =>
                (string.IsNullOrWhiteSpace(email) || log.Email.ToLower().Contains(email)) &&
                (selectedStatus == "Tất cả" || log.TrangThai == selectedStatus)
            ).ToList();

            logDataGrid.ItemsSource = filtered;
        }

        private void OnUnblockAccountClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string email)
            {
                var confirm = MessageBox.Show($"Bạn có chắc muốn mở khóa tài khoản {email} không?",
                                              "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm == MessageBoxResult.Yes)
                {
                    logService.MoKhoaTaiKhoan(email, "Đang làm việc");
                    MessageBox.Show("Đã mở khóa!", "Thông báo");
                    RefreshLogs();
                }
            }
        }

        private void RefreshLogs()
        {
            allLogs = logService.GetLogData();
            logDataGrid.ItemsSource = allLogs;
        }

        private void btnUnblock_Loaded(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.DataContext is LoginLogDTO data)
            {
                btn.Visibility = data.TrangThai == "Bị khóa" ? Visibility.Visible : Visibility.Collapsed;
            }
        }


    }
}
