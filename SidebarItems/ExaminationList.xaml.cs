using ClinicManagement.InSidebarItems;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ClinicManagement.SidebarItems
{
    /// <summary>
    /// Interaction logic for ExaminationList.xaml
    /// </summary>
    public partial class ExaminationList : UserControl
    {
        private DateTime thoiDiem; // Thời điểm hiện tại, dùng khi mới mở usercontrol ExaminationList
        private DateTime thoiDiemTiepNhan;  //Thời điểm khi tiếp nhận 1 bệnh nhân nào đó
        private DateTime thoiDiemKham;  //Thời điểm khi khám bệnh cho 1 bệnh nhân nào đó
        private string connectionString = "Data Source=LAPTOP-2FUIJHRN;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public ExaminationList()
        {
            InitializeComponent();
            thoiDiem = DateTime.Now;
            dpNgayKham.SelectedDate = thoiDiem.Date;   // Chọn ngày hôm nay trên DatePicker
            LoadDSTiepNhan(thoiDiem.Date);
        }


        private void LoadDSTiepNhan(DateTime ThoiDiemDangXet)
        {
            string querry = @"
                                SELECT 
                                    BN.ID_BenhNhan, BN.HoTenBN, BN.NgaySinh, BN.GioiTinh, TN.ThoiGianTiepNhan, TN.ID_NhanVien
                                FROM DANHSACHTIEPNHAN TN JOIN BENHNHAN BN ON TN.ID_BenhNhan = BN.ID_BenhNhan
                                WHERE TN.Is_Deleted = 0 AND CAST (TN.ThoiGianTiepNhan AS DATE) = @NgayKham
                                ORDER BY TN.ThoiGianTiepNhan ASC
                            ";
            string QDquerry = "SELECT GiaTri FROM QUI_DINH WHERE TenQuiDinh = 'SoLuongPhieuKhamToiDa'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(querry, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@NgayKham", ThoiDiemDangXet.Date);
                    SqlCommand cmd = new SqlCommand(QDquerry, con);
                    var SLBNMax = cmd.ExecuteScalar();
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dt.Columns.Add("STT", typeof(int));
                    for (int i = 0; i < dt.Rows.Count; i++)
                        dt.Rows[i]["STT"] = i + 1;

                    // Cột tạm ẩn để kiểm tra có phiếu khám hay chưa
                    dt.Columns.Add("DaCoPhieuKham", typeof(bool));
                    dt.Columns.Add("ThoiGianKhamBenh", typeof(DateTime));

                    foreach (DataRow row in dt.Rows)
                    {
                        string idBenhNhan = row["ID_BenhNhan"].ToString();
                        DateTime thoiGian = (DateTime)row["ThoiGianTiepNhan"];

                        string checkQuery = "SELECT COUNT(*) FROM PHIEUKHAM WHERE ID_BenhNhan = @ID_BenhNhan AND ThoiGianTiepNhan = @NgayKham";

                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@ID_BenhNhan", idBenhNhan);
                            checkCmd.Parameters.AddWithValue("@NgayKham", thoiGian);
                            int count = (int)checkCmd.ExecuteScalar();
                            row["DaCoPhieuKham"] = count > 0;
                        }

                        // Nếu có phiếu khám thì lấy thời gian khám
                        if ((bool)row["DaCoPhieuKham"])
                        {
                            string phieuKhamQuery = "SELECT NgayKham FROM PHIEUKHAM WHERE ID_BenhNhan = @ID_BenhNhan AND ThoiGianTiepNhan = @NgayKham";
                            using (SqlCommand phieuKhamCmd = new SqlCommand(phieuKhamQuery, con))
                            {
                                phieuKhamCmd.Parameters.AddWithValue("@ID_BenhNhan", idBenhNhan);
                                phieuKhamCmd.Parameters.AddWithValue("@NgayKham", thoiGian);
                                DateTime thoiGianKham = (DateTime)phieuKhamCmd.ExecuteScalar();
                                row["ThoiGianKhamBenh"] = thoiGianKham;
                            }
                        }
                    }

                    lblRowCount.Content = dt.Rows.Count.ToString();  //Số lượng bệnh nhân đã tiếp nhận
                    lblMax.Content = SLBNMax;                           //Số lượng bệnh nhân tối đa
                    dgTiepNhan.ItemsSource = dt.DefaultView;

                    // Bật/tắt nút tương ứng theo DaCoPhieuKham để nếu chưa tạo phiếu khám thì chỉ được tạo không được xem và ngược lại
                    dgTiepNhan.Dispatcher.InvokeAsync(() =>
                    {
                        for (int i = 0; i < dgTiepNhan.Items.Count; i++)
                        {
                            var item = dgTiepNhan.Items[i] as DataRowView;
                            if (item == null) continue;

                            bool daCoPhieu = (bool)item["DaCoPhieuKham"];

                            var row = dgTiepNhan.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                            if (row != null)
                            {
                                var btnCreate = FindVisualChild<Button>(row, "btn_createExamForm");
                                var btnView = FindVisualChild<Button>(row, "btn_viewExamForm");

                                if (btnCreate != null)
                                    btnCreate.IsEnabled = !daCoPhieu;

                                if (btnView != null)
                                    btnView.IsEnabled = daCoPhieu;
                            }
                        }
                    }, DispatcherPriority.Loaded);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild && (child as FrameworkElement)?.Name == name)
                    return tChild;

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }



        private void btn_addPatientToExam_Click(object sender, RoutedEventArgs e)
        {
            thoiDiemTiepNhan = DateTime.Now;
            lblNgayHienTai.Content = thoiDiemTiepNhan.ToString("dd/MM/yyyy HH:mm");
            AddPatientPopup.IsOpen = true;

            tbMaBN.Text = "";
            tbMaNV.Text = "";
            AddPatientPopup.HorizontalOffset = 500;
            AddPatientPopup.VerticalOffset = 300;
        }
        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            AddPatientPopup.IsOpen = false;
        }

        private void btnXacNhan_Click(object sender, RoutedEventArgs e)
        {
            string maBN = tbMaBN.Text.Trim();
            string maNV = tbMaNV.Text.Trim();

            if (string.IsNullOrEmpty(maBN) || string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bệnh nhân và nhân viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "INSERT INTO DANHSACHTIEPNHAN (ID_BenhNhan, ID_NhanVien, ThoiGianTiepNhan) VALUES (@ID_BenhNhan, @ID_NhanVien, @ThoiGianTiepNhan)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ID_BenhNhan", maBN);
                    cmd.Parameters.AddWithValue("@ID_NhanVien", maNV);
                    cmd.Parameters.AddWithValue("@ThoiGianTiepNhan", thoiDiemTiepNhan);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Thêm bệnh nhân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadDSTiepNhan(thoiDiemTiepNhan);
                        //LoadDSTiepNhan(thoiDiem);

                        AddPatientPopup.IsOpen = false;
                    }
                    else
                    {
                        MessageBox.Show("Thêm bệnh nhân thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi:\n" + ex.Message, "Thêm bệnh nhân thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btn_editPatientFromExam_Click(object sender, RoutedEventArgs e)
        {
            thoiDiemTiepNhan = DateTime.Now;
            lblEditNgayHienTai.Content = thoiDiemTiepNhan.ToString("dd/MM/yyyy");

            var selectedRow = dgTiepNhan.SelectedItem as DataRowView;

            if (selectedRow == null)
            {
                MessageBox.Show("Vui lòng chọn một hồ sơ tiếp nhận để chỉnh sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Xác nhận trước khi sửa
            var result = MessageBox.Show("Bạn có chắc chắn muốn sửa hồ sơ này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            EditPatientPopup.IsOpen = true;

            string editIdBenhNhan = selectedRow["ID_BenhNhan"].ToString();
            string editIdNhanVien = selectedRow["ID_NhanVien"].ToString();
            tbEditMaBN.Text = editIdBenhNhan;
            tbEditMaNV.Text = editIdNhanVien;
            EditPatientPopup.HorizontalOffset = 500;
            EditPatientPopup.VerticalOffset = 300;
        }

        private void btnEditXacNhan_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dòng đang chọn trong DataGrid
            var selectedRow = dgTiepNhan.SelectedItem as DataRowView;

            string editIdBenhNhan = selectedRow["ID_BenhNhan"].ToString();
            DateTime editThoiGianTiepNhan = (DateTime)selectedRow["ThoiGianTiepNhan"];

            string maBN = tbEditMaBN.Text.Trim();
            string maNV = tbEditMaNV.Text.Trim();

            if (string.IsNullOrEmpty(maBN) || string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bệnh nhân và nhân viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "UPDATE DANHSACHTIEPNHAN SET ID_BenhNhan = @ID_BenhNhan, ID_NhanVien = @ID_NhanVien, ThoiGianTiepNhan = @ThoiGianTiepNhan WHERE ID_BenhNhan = @editIdBenhNhan AND ThoiGianTiepNhan = @editThoiGianTiepNhan";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ID_BenhNhan", maBN);
                    cmd.Parameters.AddWithValue("@ID_NhanVien", maNV);
                    cmd.Parameters.AddWithValue("@ThoiGianTiepNhan", thoiDiem);
                    cmd.Parameters.AddWithValue("@editThoiGianTiepNhan", editThoiGianTiepNhan);
                    cmd.Parameters.AddWithValue("@editIdBenhNhan", editIdBenhNhan);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sửa hồ sơ tiếp nhận bệnh nhân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadDSTiepNhan(thoiDiemTiepNhan);
                        //LoadDSTiepNhan(thoiDiem);

                        EditPatientPopup.IsOpen = false;
                    }
                    else
                    {
                        MessageBox.Show("Sửa bệnh nhân thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi:\n" + ex.Message, "Sửa bệnh nhân thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEditHuy_Click(object sender, RoutedEventArgs e)
        {
            EditPatientPopup.IsOpen = false;
        }
        private void btn_deletePatientFromExam_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dòng đang chọn trong DataGrid
            var selectedRow = dgTiepNhan.SelectedItem as DataRowView;

            if (selectedRow == null)
            {
                MessageBox.Show("Vui lòng chọn một bệnh nhân để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Xác nhận trước khi xóa
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa bệnh nhân này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            string idBenhNhan = selectedRow["ID_BenhNhan"].ToString();
            DateTime thoiGianTiepNhan = (DateTime)selectedRow["ThoiGianTiepNhan"];

            // Kết nối database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string querry = "UPDATE DANHSACHTIEPNHAN SET Is_Deleted = 1 WHERE ID_BenhNhan = @ID_BenhNhan AND ThoiGianTiepNhan = @thoiGianTiepNhan";

                SqlCommand cmd = new SqlCommand(querry, conn);
                cmd.Parameters.AddWithValue("@ID_BenhNhan", idBenhNhan);
                cmd.Parameters.AddWithValue("@thoiGianTiepNhan", thoiGianTiepNhan);
                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Đã xóa bệnh nhân khỏi danh sách tiếp nhận.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        if (dpNgayKham.SelectedDate.HasValue)
                            LoadDSTiepNhan(dpNgayKham.SelectedDate.Value);
                        //LoadDSTiepNhan(thoiDiem); // Gọi lại để refresh danh sách
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy bệnh nhân cần xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi:\n" + ex.Message, "Lỗi khi xóa", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void btn_createExamForm_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.DataContext is DataRowView row)
            {
                string idBenhNhan = row["ID_BenhNhan"].ToString();
                DateTime thoiGianTiepNhan = (DateTime)row["ThoiGianTiepNhan"];

                // → Ở đây bạn có thể truyền ID vào ExaminationForm
                ExaminationForm form = new ExaminationForm(idBenhNhan, thoiGianTiepNhan);
                form.RenderTransform = new TranslateTransform();

                // Gán vào container
                var parent = this.Parent as Border;
                if (parent == null) return;

                // Tạo Storyboard để animate "this" ra trái
                var slideOut = new DoubleAnimation
                {
                    From = 0,
                    To = -this.ActualWidth,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                // Tạo animation cho UserControl mới đi vào từ bên phải
                var slideIn = new DoubleAnimation
                {
                    From = this.ActualWidth,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                // Bắt đầu animation
                var currentTransform = new TranslateTransform();
                this.RenderTransform = currentTransform;

                slideOut.Completed += (s, _) =>
                {
                    // Khi slideOut xong thì thay content
                    parent.Child = form;

                    // Animate slide-in
                    var newTransform = form.RenderTransform as TranslateTransform;
                    newTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
                };

                // Animate current control ra trái
                currentTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
            }
        }

        private void btn_viewExamForm_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.DataContext is DataRowView row)
            {
                string idBenhNhan = row["ID_BenhNhan"].ToString();
                DateTime thoiDiemKham = (DateTime)row["ThoiGianKhamBenh"];
                // → Ở đây bạn có thể truyền ID vào ExaminationForm
                ExaminationFormView form = new ExaminationFormView(idBenhNhan, thoiDiemKham);
                form.RenderTransform = new TranslateTransform();

                // Gán vào container
                var parent = this.Parent as Border;
                if (parent == null) return;

                // Tạo Storyboard để animate "this" ra trái
                var slideOut = new DoubleAnimation
                {
                    From = 0,
                    To = -this.ActualWidth,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                // Tạo animation cho UserControl mới đi vào từ bên phải
                var slideIn = new DoubleAnimation
                {
                    From = this.ActualWidth,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                // Bắt đầu animation
                var currentTransform = new TranslateTransform();
                this.RenderTransform = currentTransform;

                slideOut.Completed += (s, _) =>
                {
                    // Khi slideOut xong thì thay content
                    parent.Child = form;

                    // Animate slide-in
                    var newTransform = form.RenderTransform as TranslateTransform;
                    newTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
                };

                // Animate current control ra trái
                currentTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
            }
        }

        private void dpNgayKham_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpNgayKham.SelectedDate.HasValue)
            {
                DateTime ngayChon = dpNgayKham.SelectedDate.Value;
                LoadDSTiepNhan(ngayChon);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            double radius = 20;

            // Ensure 'Root' is defined and refers to the root container of the UserControl
            var rootElement = this.Content as FrameworkElement;
            if (rootElement != null)
            {
                var clipRect = new RectangleGeometry(new Rect(0, 0, rootElement.ActualWidth, rootElement.ActualHeight), radius, radius);
                rootElement.Clip = clipRect;
            }
        }
    }
}
