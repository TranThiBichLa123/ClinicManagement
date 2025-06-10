using BLL;
using DTO;
using FontAwesome.Sharp;
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
        private ExaminationListBLL bll = new ExaminationListBLL();
        private string Account;
        private readonly PhanQuyenBLL phanQuyenBLL = new PhanQuyenBLL();
        private readonly LoginLogBLL loginLogBLL = new LoginLogBLL();
        public ExaminationList() { }
        public ExaminationList(string userEmail)
        {
            InitializeComponent();
            thoiDiem = DateTime.Now;
            dpNgayKham.SelectedDate = thoiDiem.Date;   // Chọn ngày hôm nay trên DatePicker
            LoadDSTiepNhan(thoiDiem.Date);
            // Load quyền
            Account = userEmail;
            int nhomQuyen = phanQuyenBLL.LayNhomTheoEmail(Account);
            var danhSachQuyen = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(nhomQuyen);

            PhanQuyenHelper.DanhSachQuyen = danhSachQuyen;
            UserSession.Email = Account;
            UserSession.NhomQuyen = nhomQuyen;
            UserSession.DanhSachChucNang = danhSachQuyen;
        }

        private bool HasPermission(int chucNangId)
        {
            return UserSession.DanhSachChucNang.Contains(chucNangId);
        }

        private bool DenyIfNoPermission(int chucNangId)
        {
            if (!HasPermission(chucNangId))
            {
                MessageBox.Show("Bạn không có quyền thực hiện chức năng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }
        private void LoadDSTiepNhan(DateTime ThoiDiemDangXet)
        {
            int idNhanVien = new PhanQuyenBLL().LayIDNhanVienTheoEmail(UserSession.Email);
            bool coQuyen24 = UserSession.DanhSachChucNang.Contains(24);

            DataTable dt;

            if (coQuyen24)
            {
                dt = bll.GetDanhSachTiepNhan(ThoiDiemDangXet.Date); // toàn bộ
            }
            else
            {
                dt = bll.GetDanhSachTiepNhanTheoNhanVien(ThoiDiemDangXet.Date, idNhanVien); // lọc theo nhân viên
            }

            //var dt = bll.GetDanhSachTiepNhan(ThoiDiemDangXet.Date);
            int td = bll.GetSLBNMax();
            int ht = bll.GetSLBNTrongNgay(ThoiDiemDangXet.Date);

            dt.Columns.Add("STT", typeof(int));
            for (int i = 0; i < dt.Rows.Count; i++)
                dt.Rows[i]["STT"] = i + 1;

            dt.Columns.Add("DaCoPhieuKham", typeof(bool));
            dt.Columns.Add("TrangThai", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                int idTiepNhan = (int)row["ID_TiepNhan"];

                if (bll.CheckDaCoPK(idTiepNhan))
                {
                    row["DaCoPhieuKham"] = true;
                    row["TrangThai"] = "Đã khám";
                }
                else
                {
                    row["DaCoPhieuKham"] = false;
                    row["TrangThai"] = "Chưa khám";
                }
            }

            lblRowCount.Content = ht.ToString();
            lblMax.Content = td.ToString();
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
            if (DenyIfNoPermission(13)) return;
           
            DateTime selectedDate = (DateTime)dpNgayKham.SelectedDate;
            DateTime now = DateTime.Now;
            if (selectedDate.Date < now.Date)
            {
                MessageBox.Show("Không thể tiếp nhận từ quá khứ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (selectedDate.Date > now.Date)
            {
                MessageBox.Show("Không thể tiếp nhận tới tương lai!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int td = bll.GetSLBNMax();
            int ht = bll.GetSLBNTrongNgay(selectedDate.Date);

            if (ht == td)
            {
                MessageBox.Show("Đã đủ số lượng bệnh nhân tiếp nhận tối đa trong ngày, không thể tiếp nhận thêm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                
                thoiDiemTiepNhan = selectedDate;
                lblNgayHienTai.Content = thoiDiemTiepNhan.ToString("dd/MM/yyyy");
                AddPatientPopup.IsOpen = true;

                tbMaBN.Text = "";
                tbMaNV.Text = "";
                tbCaTN.Text = "";
                AddPatientPopup.HorizontalOffset = 500;
                AddPatientPopup.VerticalOffset = 300;
            }
        }
        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            AddPatientPopup.IsOpen = false;
        }

        private void btnXacNhan_Click(object sender, RoutedEventArgs e)
        {
            string maBN = tbMaBN.Text.Trim();
            string maNV = tbMaNV.Text.Trim();
            string caTN = tbCaTN.Text.Trim();

            if (string.IsNullOrEmpty(maBN) || string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bệnh nhân và nhân viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int ins = bll.InsertTiepNhan(maBN, maNV, thoiDiemTiepNhan.Date, caTN);

            if (ins > 0)
            {
                MessageBox.Show("Thêm bệnh nhân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadDSTiepNhan(thoiDiemTiepNhan);
                AddPatientPopup.IsOpen = false;
                loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã thêm một phiếu tiếp nhận");
            }
            else
            {
                MessageBox.Show("Thêm bệnh nhân thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_editPatientFromExam_Click(object sender, RoutedEventArgs e)
        {
            if (DenyIfNoPermission(18)) return;
            loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Truy cập quản lý nhập hàng");
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

            int idTN = (int)selectedRow["ID_TiepNhan"];
            if (bll.CheckDaCoPK(idTN))
            {
                MessageBox.Show("Bệnh nhân này đã được khám, không thể chỉnh sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                EditPatientPopup.IsOpen = true;

                string editIdBenhNhan = selectedRow["ID_BenhNhan"].ToString();
                string editIdNhanVien = selectedRow["ID_NhanVien"].ToString();
                string editCaTN = selectedRow["CaTN"].ToString();
                string editNgayTN = selectedRow["NgayTN"].ToString();

                tbEditMaBN.Text = editIdBenhNhan;
                tbEditMaNV.Text = editIdNhanVien;
                tbEditCaTN.Text = editCaTN;

                EditPatientPopup.HorizontalOffset = 500;
                EditPatientPopup.VerticalOffset = 300;
            }
        }

        private void btnEditXacNhan_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dòng đang chọn trong DataGrid
            var selectedRow = dgTiepNhan.SelectedItem as DataRowView;

            string editIdBenhNhan = selectedRow["ID_BenhNhan"].ToString();
            DateTime editNgayTiepNhan = (DateTime)selectedRow["NgayTN"];
            string editCaTN = selectedRow["CaTN"].ToString();
            string editIDNhanVien = selectedRow["ID_NhanVien"].ToString();

            string maBN = tbEditMaBN.Text.Trim();
            string maNV = tbEditMaNV.Text.Trim();
            string caTN = tbEditCaTN.Text.Trim();
            DateTime ngayTN = thoiDiemTiepNhan.Date;

            if (string.IsNullOrEmpty(maBN) || string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bệnh nhân và nhân viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int upd = bll.UpdateTiepNhan(maBN, maNV, ngayTN, caTN, editNgayTiepNhan, editCaTN, editIdBenhNhan, editIDNhanVien);
            if (upd > 0)
            {
                MessageBox.Show("Sửa hồ sơ tiếp nhận bệnh nhân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadDSTiepNhan(thoiDiemTiepNhan);
                //LoadDSTiepNhan(thoiDiem);

                EditPatientPopup.IsOpen = false;
                loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã sửa một phiếu tiếp nhận");
            }
            else
            {
                MessageBox.Show("Sửa bệnh nhân thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditHuy_Click(object sender, RoutedEventArgs e)
        {
            EditPatientPopup.IsOpen = false;
        }
        private void btn_deletePatientFromExam_Click(object sender, RoutedEventArgs e)
        {
            if (DenyIfNoPermission(22)) return;
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

            string delIdBenhNhan = selectedRow["ID_BenhNhan"].ToString();
            DateTime delNgayTiepNhan = (DateTime)selectedRow["NgayTN"];
            string delCaTN = selectedRow["CaTN"].ToString();
            string delIDNhanVien = selectedRow["ID_NhanVien"].ToString();

            int idTN = (int)selectedRow["ID_TiepNhan"];
            if (bll.CheckDaCoPK(idTN))
            {
                MessageBox.Show("Bệnh nhân này đã được khám, không thể xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                int del = bll.DeleteTiepNhan(delIdBenhNhan, delNgayTiepNhan, delCaTN, delIDNhanVien);
                if (del > 0)
                {
                    MessageBox.Show("Đã xóa bệnh nhân khỏi danh sách tiếp nhận.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (dpNgayKham.SelectedDate.HasValue)
                        LoadDSTiepNhan(dpNgayKham.SelectedDate.Value);
                    loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã xóa một phiếu tiếp nhận");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy bệnh nhân cần xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btn_createExamForm_Click(object sender, RoutedEventArgs e)
        {
            loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã tạo một phiếu khám");
            var btn = sender as Button;
            if (btn?.DataContext is DataRowView row)
            {
                int idTN = (int)row["ID_TiepNhan"];
                string idBN = row["ID_BenhNhan"].ToString();
                // → Ở đây bạn có thể truyền ID vào ExaminationForm
                ExaminationForm form = new ExaminationForm(idBN, idTN, Account);
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
            loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã xem một phiếu khám");
            var btn = sender as Button;
            if (btn?.DataContext is DataRowView row)
            {
                string idBenhNhan = row["ID_BenhNhan"].ToString();
                int idTN = (int)row["ID_TiepNhan"];
                // → Ở đây bạn có thể truyền ID vào ExaminationForm
                ExaminationFormView form = new ExaminationFormView(idBenhNhan, idTN, Account, ExaminationFormView.PreviousScreen.ExaminationList);
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
            
        }
    }
}
