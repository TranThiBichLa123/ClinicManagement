using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BLL;
using DTO;

namespace ClinicManagement.SidebarItems
{
    public partial class ReportView : UserControl
    {
        private ReportBLL reportBLL = new ReportBLL();

        public ReportView()
        {
            InitializeComponent();
            LoadComboBoxValues();
        }

        private void ReportView_Loaded(object sender, RoutedEventArgs e)
        {
            // Auto-select current month and year
            cbThang_DoanhThu.SelectedItem = DateTime.Now.Month;
            cbNam_DoanhThu.SelectedItem = DateTime.Now.Year;

            cbThang_Thuoc.SelectedItem = DateTime.Now.Month;
            cbNam_Thuoc.SelectedItem = DateTime.Now.Year;
        }

        private void LoadComboBoxValues()
        {
            for (int i = 1; i <= 12; i++)
            {
                cbThang_DoanhThu.Items.Add(i);
                cbThang_Thuoc.Items.Add(i);
            }

            for (int year = DateTime.Now.Year - 5; year <= DateTime.Now.Year + 1; year++)
            {
                cbNam_DoanhThu.Items.Add(year);
                cbNam_Thuoc.Items.Add(year);
            }
        }
        private bool KiemTraThangChuaKetThuc(int thang, int nam)
        {
            DateTime now = DateTime.Now;
            return (thang == now.Month && nam == now.Year && now.Day < DateTime.DaysInMonth(nam, thang));
        }
        private void btnXemBaoCaoDoanhThu_Click(object sender, RoutedEventArgs e)
        {
            if (cbThang_DoanhThu.SelectedItem != null && cbNam_DoanhThu.SelectedItem != null)
            {
                int thang = int.Parse(cbThang_DoanhThu.SelectedItem.ToString());
                int nam = int.Parse(cbNam_DoanhThu.SelectedItem.ToString());

                if (KiemTraThangChuaKetThuc(thang, nam))
                {
                    MessageBox.Show($"Tháng {thang}/{nam} chưa kết thúc. Vui lòng xem lại sau khi kết thúc tháng.",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Binding chi tiết vào DataGrid
                var chiTiet = reportBLL.GetChiTietDoanhThu(thang, nam);
                dgDoanhThu.ItemsSource = chiTiet;

                // Binding tổng doanh thu
                var tong = reportBLL.GetTongDoanhThu(thang, nam);
                txtTongDoanhThu.Text = tong != null
                    ? $"Tổng doanh thu tháng {thang}/{nam}: {tong.TongDoanhThu:N0} VNĐ"
                    : $"Không có dữ liệu cho tháng {thang}/{nam}";
            }
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Xử lý gì đó nếu cần, hoặc để trống
        }

        private void btnXemBaoCaoThuoc_Click(object sender, RoutedEventArgs e)
        {
            if (cbThang_Thuoc.SelectedItem != null && cbNam_Thuoc.SelectedItem != null)
            {
                int thang = int.Parse(cbThang_Thuoc.SelectedItem.ToString());
                int nam = int.Parse(cbNam_Thuoc.SelectedItem.ToString());

                if (KiemTraThangChuaKetThuc(thang, nam))
                {
                    MessageBox.Show($"Tháng {thang}/{nam} chưa kết thúc. Vui lòng xem lại sau khi kết thúc tháng.",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var dsBaoCao = reportBLL.GetBaoCaoSuDungThuoc(thang, nam);
                dgThuoc.ItemsSource = dsBaoCao;
            }
        }
    }
}
