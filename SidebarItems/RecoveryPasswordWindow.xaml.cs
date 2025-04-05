using System.Net;
using System.Net.Mail;
using System.Windows;
using ClinicManagement.SidebarItems;
namespace ClinicManagement.SidebarItems
{
    public partial class RecoveryPasswordWindow : Window
    {
        string randomCode;
        private string email;
        public RecoveryPasswordWindow(string code, string email)
        {
            InitializeComponent();
            this.randomCode = code;
            this.email = email;
        }

        private void ConfirmCode_Click(object sender, RoutedEventArgs e)
        {
            if (randomCode == textBoxCode.Text)
            {
                ResetPassword rp = new ResetPassword(this.email);
                this.Hide();
                rp.Show();
            }
            else
            {
                MessageBox.Show("Sai mã xác nhận", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

            if (textBoxCode.Text.Length > 0)
                textBlockHint.Visibility = Visibility.Collapsed; // Ẩn gợi ý khi có mật khẩu
            else
                textBlockHint.Visibility = Visibility.Visible; // Hiện gợi ý khi ô trống

        }
    }
}