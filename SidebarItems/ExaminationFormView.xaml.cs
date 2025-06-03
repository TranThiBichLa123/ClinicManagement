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
        private string connectionString = "Data Source=KOROBE\\SQLEXPRESS;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public ExaminationFormView(string idBenhNhan, int idTiepNhan)
        {
            InitializeComponent();
            this.idBenhNhan = idBenhNhan;
            this.idTiepNhan = idTiepNhan;
            lblMaBN.Content = idBenhNhan;

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
                lblNgayKham.Content = "Ca " + pk["CaKham"] + " Ngày " + pk["NgayTN"];

                var toa = bll.GetToaThuocTheoPhieuKham(idPK);
                dgToaThuoc.ItemsSource = toa.DefaultView;
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin phiếu khám của bệnh nhân này!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AnimateBack()
        {
            // Tạo hiệu ứng slide-out sang phải cho form hiện tại
            var currentTransform = new TranslateTransform();
            this.RenderTransform = currentTransform;

            var slideOut = new DoubleAnimation
            {
                From = 0,
                To = this.ActualWidth,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Khi slideOut xong, thay thế bằng ExaminationList
            slideOut.Completed += (s, _) =>
            {
                var parent = this.Parent as Border;
                if (parent != null)
                {
                    var list = new SidebarItems.ExaminationList();
                    list.RenderTransform = new TranslateTransform { X = -this.ActualWidth }; // Bắt đầu từ bên trái

                    parent.Child = list;

                    var slideIn = new DoubleAnimation
                    {
                        From = -this.ActualWidth,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(300),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                    };

                    (list.RenderTransform as TranslateTransform).BeginAnimation(TranslateTransform.XProperty, slideIn);
                }
            };

            // Bắt đầu animation ra ngoài
            currentTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AnimateBack();
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

        private void btnXoaPK_Click(object sender, RoutedEventArgs e)
        {
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
            }

        }

        private void btnSuaPK_Click(object sender, RoutedEventArgs e)
        {
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
                MessageBox.Show("Phiếu khám đã được xuất hóa đơn, không thể xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ExaminationForm form = new ExaminationForm(idBN, idTN, idPK);
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