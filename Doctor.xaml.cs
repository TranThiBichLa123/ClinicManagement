using ClinicManagement.SidebarItems;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ClinicManagement
{
    public partial class Doctor : Window
    {
        public void LoadUserControl(UserControl userControl)
        {
            // Kiểm tra Border có chứa phần tử con hay không
            if (fContainer.Child != null)
            {
                fContainer.Child = null; // Xóa nội dung cũ
            }

            fContainer.Child = userControl; // Gán UserControl vào Border
        }
        private string Account;
        public Doctor() { } //  constructor mặc định
        public Doctor(string userEmail)
        {
            InitializeComponent();
            LoadUserControl(new DashBoard());
            Account = userEmail;
        }
        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new DashBoard());

        }

        private void btnHome_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;

        }

        private void btnHome_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnHome;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Tổng quan";
            }

        }

        private void btnBilling_MouseEnter(object sender, MouseEventArgs e)
        {

            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnBilling;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Hóa đơn";
            }
        }

        private void btnBilling_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;

        }

        private void btnBilling_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new InvoiceList(this));
        }




        private void btnPointOfSale_MouseLeave(object sender, MouseEventArgs e)
        {

            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }


        private void btnSecurity_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnSetting_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnSetting;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Logout";
            }

        }

        private void btnSetting_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }


        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }

        private void btnStaff_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnStaff;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Nhân viên";
            }
        }

        private void btnStaff_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;

        }

        private void btnStaff_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new DashBoard());


        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;

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

        private void Drug_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;

        }

        private void Drug_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnDrug;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Thuốc";
            }
        }

        private void MenuItem_Loaded(object sender, RoutedEventArgs e)
        {

        }

       
        private void btnDrug_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new DrugView());

        }


        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new Setting());


        }
        private void btnReport_MouseEnter(object sender, MouseEventArgs e)
        {

            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnReport;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Báo cáo";
            }
        }

        private void btnReport_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;

        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new ReportView());
        }

        private void btnPatient_MouseEnter(object sender, MouseEventArgs e)
        {

            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnPatient;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Bệnh nhân";
            }
        }

        private void btnPatient_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;

        }

        private void btnPatient_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new ReportView());
        }

        private void btnExam_MouseEnter(object sender, MouseEventArgs e)
        {

            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnExam;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Danh sách tiếp nhận";
            }
        }

        private void btnExam_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;

        }

        private void btnExam_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new ReportView());
        }
        private void btnRules_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnRules;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Quy định";
            }
        }

        private void btnRules_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;

        }

        private void btnRules_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new DashBoard());


        }
        private void btnAccount_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == false)
            {
                Popup.PlacementTarget = btnAccount;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Phân quyền";
            }
        }

        private void btnAccount_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;

        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            LoadUserControl(new DashBoard());


        }
    }
}
