using System.Windows;
using System.Net;
using System.Net.Mail;
using System;

namespace ClinicManagement.SidebarItems
{
    public partial class ForgotPassword : Window
    {
        
        public static string to;
        public ForgotPassword()
        {
            InitializeComponent();
        }


        private void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            string from = "23520827@gm.uit.edu.vn";
            string pass = "velc ulct yoxj cxxb"; // App Password từ Gmail (không phải mật khẩu Gmail thật)
            string to = textBoxEmail.Text; // Email người nhận nhập vào

            // Tạo mã xác nhận ngẫu nhiên
            Random rand = new Random();
            string randomCode = rand.Next(100000, 999999).ToString(); // mã 6 chữ số

            string subject = "Mã xác nhận quên mật khẩu";
            string messageBody = "Mã xác nhận của bạn là: " + randomCode;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(from, "Hệ thống hỗ trợ mật khẩu");
            message.To.Add(to);
            message.Subject = subject;
            message.Body = messageBody;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(from, pass);

            try
            {
                smtp.Send(message);
                MessageBox.Show("Mã xác nhận đã được gửi đến email của bạn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Truyền mã và email sang cửa sổ xác minh
                RecoveryPasswordWindow recoveryPasswordWindow = new RecoveryPasswordWindow(randomCode, to);
                recoveryPasswordWindow.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi email: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}