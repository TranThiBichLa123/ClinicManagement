using BLL;
using DTO;
using System.Windows;
using System.Windows.Input;
using System.Data.SqlClient;
namespace ClinicManagement.SidebarItems
{
    public static class GlobalData
    {
        public static string LoggedInPassword { get; set; }
    }

    public partial class Login : Window
    {
        Account acc = new Account();
        AccountBLL accBLL = new AccountBLL();

       
        public Login()
        {
            InitializeComponent();
        }

        private void SignUp_Click(object sender, MouseButtonEventArgs e)
        {

            ForgotPassword enterEmailWindow = new ForgotPassword(); 
            enterEmailWindow.Show(); 

        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password.Length > 0)
                textBlockHint.Visibility = Visibility.Collapsed; // Ẩn gợi ý khi có mật khẩu
            else
                textBlockHint.Visibility = Visibility.Visible; // Hiện gợi ý khi ô trống


        }

        private void HidePassword_Checked(object sender, RoutedEventArgs e)
        {

            passwordBox.Visibility = Visibility.Visible;
            textBoxPassword.Visibility = Visibility.Collapsed;

        }

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {

            textBoxPassword.Text = passwordBox.Password;
            textBoxPassword.Visibility = Visibility.Visible;
            passwordBox.Visibility = Visibility.Collapsed;

        }

        private void Password_Checked(object sender, MouseButtonEventArgs e)
        {

            ForgotPassword passwordWindow = new ForgotPassword(); 
            passwordWindow.Show(); 

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
             
            WindowState = WindowState.Minimized;
    }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;

    }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Account acc = new Account();
            acc.Email = textBoxEmail.Text;
            //  Lấy mật khẩu từ đúng control đang hiển thị
            if (textBoxPassword.Visibility == Visibility.Visible)
            {
                acc.MatKhau = textBoxPassword.Text;
            }
            else
            {
                acc.MatKhau = passwordBox.Password;
            }

            string userRole;
            string result = accBLL.CheckLogin(acc, out userRole); 

            switch (result)
            {
                case "request_taikhoan":
                    MessageBox.Show("Email không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case "request_password":
                    MessageBox.Show("Mật khẩu không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case "invalid_login":
                    MessageBox.Show("Email hoặc mật khẩu không chính xác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case "success":
                    GlobalData.LoggedInPassword = acc.MatKhau;   
                    if (userRole == "1")
                    {
                        Doctor dashboard = new Doctor(acc.Email);
                        dashboard.Show();
                    }
                    else if (userRole == "2")
                    {
                        Cashier csher = new Cashier(acc.Email); 
                        csher.Show();
                    }
                    else if (userRole == "3")
                    {
                        Receptionist rcpt = new Receptionist(acc.Email); 
                        rcpt.Show();
                    }
                    
                    this.Close();
                    break;
            }
        }

        private void textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (textBoxPassword.Text.Length > 0)
                textBlockHint.Visibility = Visibility.Collapsed; // Ẩn gợi ý khi có mật khẩu
            else
                textBlockHint.Visibility = Visibility.Visible; // Hiện gợi ý khi ô trống
        }
    }
}
