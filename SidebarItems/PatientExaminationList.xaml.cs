using ClinicManagement.SidebarItems;
using DTO;
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
using static ClinicManagement.SidebarItems.PatientList;
using BLL;

namespace ClinicManagement.SidebarItems
{
    public partial class PatientExaminationList : UserControl
    {
        private string ID_BenhNhan;
        private ExaminationBLL examinationBLL;
        private string Account;
        public PatientExaminationList(string ID_BenhNhan, string userEmail)
        {
            InitializeComponent();
            this.ID_BenhNhan = ID_BenhNhan;
            DataContext = this;
            Account = userEmail;
            examinationBLL = new ExaminationBLL();
            LoadPatientInfo(ID_BenhNhan);
            LoadPhieuKham();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            double radius = 20;

            var rootElement = this.Content as FrameworkElement;
            if (rootElement != null)
            {
                var clipRect = new RectangleGeometry(new Rect(0, 0, rootElement.ActualWidth, rootElement.ActualHeight), radius, radius);
                rootElement.Clip = clipRect;
            }
        }
        private BenhNhan GetBenhNhanById(string id)
        {
            return examinationBLL.GetBenhNhanById(id);
        }
        private void LoadPatientInfo(string id)
        {
            BenhNhan patient = GetBenhNhanById(id);

            if (patient != null)
            {
                txtIDPatient.Text = patient.ID_BenhNhan.ToString();
                txtPatientName.Text = patient.HoTenBN;
                txtPatientSex.Text = patient.GioiTinh;
                txtPatientDate.Text = patient.NgaySinh.ToString("dd/MM/yyyy");

                if (patient.GioiTinh.ToLower() == "nam")
                {
                    iconGender.Kind = MaterialDesignThemes.Wpf.PackIconKind.GenderMale;
                }
                else if (patient.GioiTinh.ToLower() == "nữ")
                {
                    iconGender.Kind = MaterialDesignThemes.Wpf.PackIconKind.GenderFemale;
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin bệnh nhân.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void LoadPhieuKham()
        {
            List<PhieuKham> danhSachPhieu = examinationBLL.LoadPhieuKham(ID_BenhNhan);
            dgPhieuKham.ItemsSource = danhSachPhieu;
            txtExaminationCount.Text = danhSachPhieu.Count.ToString();
        }

        private void btn_ViewExamination_Click(object sender, RoutedEventArgs e)
        {
            var selectedPhieu = ((FrameworkElement)sender).DataContext as PhieuKham;
            if (selectedPhieu == null)
                return;

            int idPhieuKham = selectedPhieu.ID_PhieuKham;

            // Correcting the fourth argument to match the expected enum type  
            ExaminationFormView form = new ExaminationFormView(ID_BenhNhan, idPhieuKham, false, Account, ExaminationFormView.PreviousScreen.PatientExaminationList);
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

        private void LoadDanhSachThuoc(int idPhieuKham)
        {
            List<Thuoc> danhSachThuoc = new List<Thuoc>();
            try
            {
                danhSachThuoc = examinationBLL.LoadDanhSachThuoc(idPhieuKham);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách thuốc: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            dgDSThuoc.ItemsSource = danhSachThuoc;
        }

        private void ShowExaminationPopup(int idPhieuKham)
        {
            try
            {
                object[] objects = examinationBLL.ShowExaminationPopup(idPhieuKham);
                if (objects[0] != null && objects[1] != null && objects[2] != null)
                {
                    txtIDPhieuKham.Text = idPhieuKham.ToString();
                    txtCAKham.Text = objects[5].ToString();
                    txtTrieuChung.Text = objects[1].ToString();
                    txtTienKham.Text = Convert.ToDecimal(objects[3]).ToString("N0");
                    txtTongTienThuoc.Text = Convert.ToDecimal(objects[4]).ToString("N0");
                    txtTenLoaiBenh.Text = objects[2].ToString();

                    ExaminationPopup.IsOpen = true;
                    LoadDanhSachThuoc(idPhieuKham);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải chi tiết phiếu khám: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            // Tạo control cũ (PatientList)
            PatientList previous = new PatientList(UserSession.Email);
            previous.RenderTransform = new TranslateTransform();

            // Lấy parent container
            var parent = this.Parent as Border;
            if (parent == null) return;

            // Animation slide out hiện tại sang phải
            var slideOut = new DoubleAnimation
            {
                From = 0,
                To = this.ActualWidth,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Animation slide in control cũ từ trái
            var slideIn = new DoubleAnimation
            {
                From = -this.ActualWidth,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var currentTransform = new TranslateTransform();
            this.RenderTransform = currentTransform;

            slideOut.Completed += (s, _) =>
            {
                parent.Child = previous;
                var previousTransform = previous.RenderTransform as TranslateTransform;
                previousTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
            };

            currentTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
        }
        private void CancelExamination_Click(object sender, RoutedEventArgs e)
        {
            ExaminationPopup.IsOpen = false;
        }
    }
}
