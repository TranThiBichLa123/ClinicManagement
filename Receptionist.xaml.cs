using System.Windows;
using System.Windows.Input;


namespace ClinicManagement
{
    public partial class Receptionist : Window
    {
        private string Account;
        public Receptionist() { } // constructor mặc định

        public Receptionist(string userEmail)
        {
            InitializeComponent();
            Account = userEmail;

        }
        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new ClinicManagement.SidebarItems.Login();
                loginWindow.Show();

                Application.Current.MainWindow = loginWindow;

                this.Close();
            }
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {

            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;

        }

        private void btnHome_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnHome_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void btnDashboard_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnProducts_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnProductStock_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnDashboard_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void btnProducts_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void btnProductStock_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void btnOrderList_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnBilling_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnOrderList_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void btnBilling_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void btnSetting_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void btnSetting_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
