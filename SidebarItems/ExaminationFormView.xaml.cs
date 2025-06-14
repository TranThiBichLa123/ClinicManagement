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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BLL;
using DTO;
using ClinicManagement.SidebarItems;

namespace ClinicManagement.SidebarItems
{
    /// <summary>
    /// Interaction logic for ExaminationFormView.xaml
    /// </summary>
    public partial class ExaminationFormView : UserControl
    {
        private string idBenhNhan;
        private int idTiepNhan;
        private int idPK;
        private ExaminationFormViewBLL bll = new ExaminationFormViewBLL();
        private ExaminationFormDTO examinationForm = new ExaminationFormDTO();
        private readonly LoginLogBLL loginLogBLL = new LoginLogBLL();
        private string Account;
        private readonly PhanQuyenBLL phanQuyenBLL = new PhanQuyenBLL();

        public enum PreviousScreen
        {
            PatientExaminationList,
            ExaminationList
        }
        private PreviousScreen fromScreen;
        public ExaminationFormView(string idBenhNhan, int idTiepNhan, string userEmail, PreviousScreen fromScreen)
        {
            InitializeComponent();
            this.idBenhNhan = idBenhNhan;
            this.idTiepNhan = idTiepNhan;
            lblMaBN.Content = idBenhNhan;
            this.fromScreen = fromScreen;
            Account = userEmail;
            // Lấy danh sách nhóm/quyền từ email
            int nhomQuyen = phanQuyenBLL.LayNhomTheoEmail(Account);
            var danhSachQuyen = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(nhomQuyen);
            // Gán vào helper (nếu cần dùng ở nơi khác)
            PhanQuyenHelper.DanhSachQuyen = danhSachQuyen;


            UserSession.Email = Account;
            UserSession.NhomQuyen = phanQuyenBLL.LayNhomTheoEmail(UserSession.Email);
            UserSession.DanhSachChucNang = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(UserSession.NhomQuyen);

            var pk = bll.GetPhieuKham(this.idTiepNhan);
            if (pk != null)
            {
                idPK = Convert.ToInt32(pk["ID_PhieuKham"]);
                lblMaPK.Content = idPK.ToString();
                lblHoTenBN.Content = pk["HoTenBN"].ToString();
                lblTrieuChung.Content = pk["TrieuChung"].ToString();
                lblChuanDoan.Content = pk["TenLoaiBenh"].ToString();
                lblTienKham.Content = ((decimal)pk["TienKham"]).ToString("N0");
                lblTongTienThuoc.Content = ((decimal)pk["TongTienThuoc"]).ToString("N0");
                lblNgayKham.Content = pk["CaKham"] + " " + ((DateTime)pk["NgayTN"]).ToString("dd/MM/yyyy");

                var toa = bll.GetToaThuocTheoPhieuKham(idPK);
                dgToaThuoc.ItemsSource = toa.DefaultView;
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin phiếu khám của bệnh nhân này!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.fromScreen = fromScreen;
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
        public ExaminationFormView(string idBenhNhan, int idPhieuKham, bool _, string userEmail, PreviousScreen fromScreen)
        {
            InitializeComponent();
            this.idBenhNhan = idBenhNhan;
            lblMaBN.Content = idBenhNhan;
            this.fromScreen = fromScreen;
            Account = userEmail;
            // Lấy danh sách nhóm/quyền từ email
            int nhomQuyen = phanQuyenBLL.LayNhomTheoEmail(Account);
            var danhSachQuyen = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(nhomQuyen);
            // Gán vào helper (nếu cần dùng ở nơi khác)
            PhanQuyenHelper.DanhSachQuyen = danhSachQuyen;


            UserSession.Email = Account;
            UserSession.NhomQuyen = phanQuyenBLL.LayNhomTheoEmail(UserSession.Email);
            UserSession.DanhSachChucNang = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(UserSession.NhomQuyen);

            var pk = bll.GetPhieuKham(idPhieuKham, _);
            if (pk != null)
            {
                idPK = Convert.ToInt32(pk["ID_PhieuKham"]);
                lblMaPK.Content = idPK.ToString();
                lblHoTenBN.Content = pk["HoTenBN"].ToString();
                lblTrieuChung.Content = pk["TrieuChung"].ToString();
                lblChuanDoan.Content = pk["TenLoaiBenh"].ToString();
                lblTienKham.Content = ((decimal)pk["TienKham"]).ToString("N0");
                lblTongTienThuoc.Content = ((decimal)pk["TongTienThuoc"]).ToString("N0");
                lblNgayKham.Content = pk["CaKham"] + " " + ((DateTime)pk["NgayTN"]).ToString("dd/MM/yyyy");

                var toa = bll.GetToaThuocTheoPhieuKham(idPK);
                dgToaThuoc.ItemsSource = toa.DefaultView;
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin phiếu khám của bệnh nhân này!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.fromScreen = fromScreen;
        }


        private void AnimateBack()
        {
            var currentTransform = new TranslateTransform();
            this.RenderTransform = currentTransform;

            var slideOut = new DoubleAnimation
            {
                From = 0,
                To = this.ActualWidth,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            slideOut.Completed += (s, _) =>
            {
                var parent = this.Parent as Border;
                if (parent != null)
                {
                    UserControl backControl = null;

                    if (fromScreen == PreviousScreen.PatientExaminationList)
                    {
                        backControl = new SidebarItems.PatientExaminationList(this.idBenhNhan, Account); // Pass the required parameter 'ID_BenhNhan'
                    }
                    else
                    {
                        backControl = new SidebarItems.ExaminationList(Account);
                    }

                    if (backControl != null)
                    {
                        backControl.RenderTransform = new TranslateTransform { X = -this.ActualWidth };
                        parent.Child = backControl;

                        var slideIn = new DoubleAnimation
                        {
                            From = -this.ActualWidth,
                            To = 0,
                            Duration = TimeSpan.FromMilliseconds(300),
                            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                        };

                        (backControl.RenderTransform as TranslateTransform).BeginAnimation(TranslateTransform.XProperty, slideIn);
                    }
                }
            };

            currentTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AnimateBack();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnXoaPK_Click(object sender, RoutedEventArgs e)
        {
            if (DenyIfNoPermission(25)) return;
          
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu khám này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            var check = bll.CheckDaXuatHD(this.idPK);
            if (check)
            {
                MessageBox.Show("Phiếu khám đã được xuất hóa đơn, không thể xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                bll.DeleteToaThuoc(this.idPK);
                bll.XoaPhieuKham(this.idPK);
                MessageBox.Show("Đã xóa thông tin phiếu khám và toa thuốc!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                AnimateBack();
                loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã xóa một phiếu khám");
            }

        }

        private void btnSuaPK_Click(object sender, RoutedEventArgs e)
        {
            if (DenyIfNoPermission(20)) return;
           
            int idPK = this.idPK;
            string idBN = this.idBenhNhan;
            int idTN = this.idTiepNhan;
            // → Ở đây bạn có thể truyền ID vào ExaminationForm
            var result = MessageBox.Show("Bạn có chắc chắn muốn sửa phiếu khám này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            var check = bll.CheckDaXuatHD(this.idPK);
            if (check)
            {
                MessageBox.Show("Phiếu khám đã được xuất hóa đơn, không thể sửa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã sửa một phiếu khám");
                ExaminationForm form = new ExaminationForm(idBN, idTN, idPK, Account);
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
    }

}