using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System;
using FontAwesome.Sharp;

namespace ClinicManagement.SidebarItems
{
    public partial class ResetPassword : Window
    {
        private string email;
        public ResetPassword(string email)
        {
            InitializeComponent();
            this.email = email;
        }

        private void passwordBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passwordBox1.Password.Length > 0)
                textBlockHint1.Visibility = Visibility.Collapsed; // Ẩn gợi ý khi có mật khẩu
            else
                textBlockHint1.Visibility = Visibility.Visible; // Hiện gợi ý khi ô trống

        }
        
        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxPassword1.Text.Length > 0)
                textBlockHint1.Visibility = Visibility.Collapsed; // Ẩn gợi ý khi có mật khẩu
            else
                textBlockHint1.Visibility = Visibility.Visible; // Hiện gợi ý khi ô trống


        }
        
        private void TogglePasswordVisibility1_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox1.Visibility == Visibility.Visible)
            {
                textBoxPassword1.Text = passwordBox1.Password;
                passwordBox1.Visibility = Visibility.Collapsed;
                textBoxPassword1.Visibility = Visibility.Visible;
            }
            else
            {
                passwordBox1.Password = textBoxPassword1.Text;
                passwordBox1.Visibility = Visibility.Visible;
                textBoxPassword1.Visibility = Visibility.Collapsed;
            }
        }
//-----------------------------------------------------------------------------------------------------------------------------
        private void passwordBox2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (textBoxPassword2.Visibility == Visibility.Visible)
            {
                textBoxPassword2.Text = passwordBox2.Password;
            }

            textBlockHint2.Visibility = string.IsNullOrEmpty(passwordBox2.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (passwordBox2.Visibility == Visibility.Visible)
            {
                passwordBox2.Password = textBoxPassword2.Text;
            }

            textBlockHint2.Visibility = string.IsNullOrEmpty(textBoxPassword2.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TogglePasswordVisibility2_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox2.Visibility == Visibility.Visible)
            {
                // Hiện password (bằng TextBox)
                textBoxPassword2.Text = passwordBox2.Password;
                passwordBox2.Visibility = Visibility.Collapsed;
                textBoxPassword2.Visibility = Visibility.Visible;
            }
            else
            {
                // Ẩn password (bằng PasswordBox)
                passwordBox2.Password = textBoxPassword2.Text;
                passwordBox2.Visibility = Visibility.Visible;
                textBoxPassword2.Visibility = Visibility.Collapsed;
            }

        }

       // string username = sendCode.to;
        private void NewPassword_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = passwordBox1.Visibility == Visibility.Visible
    ? passwordBox1.Password
    : textBoxPassword1.Text;

            string confirmPassword = passwordBox2.Visibility == Visibility.Visible
                ? passwordBox2.Password
                : textBoxPassword2.Text;

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp.");
                return;
            }

            using (SqlConnection con = new SqlConnection(
                "Data Source=KOROBE\\SQLEXPRESS;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;TrustServerCertificate=True"))
            {
                SqlCommand cmd = new SqlCommand(@"UPDATE NHANVIEN 
        SET MatKhau = @Password 
        WHERE Email = @Email", con);

                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", newPassword);

                con.Open();
                int rows = cmd.ExecuteNonQuery();
                con.Close();

                if (rows > 0)
                    MessageBox.Show("Đặt lại mật khẩu thành công!");
                else
                    MessageBox.Show("Không tìm thấy tài khoản cần cập nhật.");
            }


        }
    }
}
