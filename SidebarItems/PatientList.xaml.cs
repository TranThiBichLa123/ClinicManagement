using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using DTO;
using BLL;

namespace ClinicManagement.SidebarItems
{
    public partial class PatientList : UserControl
    {
        private BenhNhanBLL benhnhanBLL;
        public PatientList()
        {
            InitializeComponent();
            benhnhanBLL = new BenhNhanBLL();
            LoadDSBenhNhan();
        }

        private void LoadDSBenhNhan(string nameKeyword = "", string idKeyword = "", string loaiBenhKeyword = "", string trieuChungKeyword = "", string ngayDK = "")
        {
            try
            {
                DataTable dt = benhnhanBLL.LoadPatientList(nameKeyword, idKeyword, loaiBenhKeyword, trieuChungKeyword, ngayDK);
                dgBenhNhan.ItemsSource = dt.DefaultView;
                txtPatientCount.Text = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách bệnh nhân: " + ex.Message);
            }
        }

        private void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            AddPatientPopup.IsOpen = true;
        }

        private void SavePatient_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPatientName.Text) ||
                string.IsNullOrEmpty(txtPhone.Text) ||
                string.IsNullOrEmpty(txtEmail.Text) ||
                string.IsNullOrEmpty(txtCCCD.Text) ||
                string.IsNullOrEmpty(txtAddress.Text) ||
                dpDOB.SelectedDate == null ||
                cmbGender.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo đối tượng bệnh nhân
            BenhNhan newPatient = new BenhNhan
            {
                HoTenBN = txtPatientName.Text,
                NgaySinh = dpDOB.SelectedDate.Value,
                GioiTinh = ((ComboBoxItem)cmbGender.SelectedItem).Content.ToString(),
                CCCD = txtCCCD.Text,
                DienThoai = txtPhone.Text,
                DiaChi = txtAddress.Text,
                Email = txtEmail.Text,
                NgayDK = DateTime.Now,
                Is_Deleted = false
            };

            try
            {
                bool isAdded = benhnhanBLL.AddPatientToDatabase(newPatient);

                if (isAdded)
                {
                    MessageBox.Show("Thêm bệnh nhân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Không thể thêm bệnh nhân. Vui lòng thử lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            txtPatientName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtCCCD.Clear();
            txtAddress.Clear();
            dpDOB.SelectedDate = null;
            cmbGender.SelectedIndex = -1;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            AddPatientPopup.IsOpen = false;
            ClearForm();
        }

        private DataRowView selectedPatient;
        private void btn_editPatient_Click(object sender, RoutedEventArgs e)
        {
            selectedPatient = dgBenhNhan.SelectedItem as DataRowView;

            if (selectedPatient == null)
            {
                MessageBox.Show("Vui lòng chọn một bệnh nhân để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditPatientName.Text = selectedPatient["HoTenBN"].ToString();
            EditDOB.SelectedDate = Convert.ToDateTime(selectedPatient["NgaySinh"]);
            EditPhone.Text = selectedPatient["DienThoai"].ToString();
            EditEmail.Text = selectedPatient["Email"].ToString();
            EditCCCD.Text = selectedPatient["CCCD"].ToString();
            EditAddress.Text = selectedPatient["DiaChi"].ToString();

            string gioiTinh = selectedPatient["GioiTinh"].ToString();
            foreach (ComboBoxItem item in EditGender.Items)
            {
                if ((item.Content as string) == gioiTinh)
                {
                    EditGender.SelectedItem = item;
                    break;
                }
            }

            EditPatientPopup.IsOpen = true;
        }

        private void SaveEditedPatient_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = dgBenhNhan.SelectedItem as DataRowView;
            if (selectedRow == null)
            {
                MessageBox.Show("Không tìm thấy bệnh nhân cần sửa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int idBenhNhan = Convert.ToInt32(selectedRow["ID_BenhNhan"]);

            string newName = string.IsNullOrWhiteSpace(EditPatientName.Text)
                ? selectedRow["HoTenBN"].ToString()
                : EditPatientName.Text;

            DateTime newDOB = EditDOB.SelectedDate.HasValue
                ? EditDOB.SelectedDate.Value
                : Convert.ToDateTime(selectedRow["NgaySinh"]);

            string newPhone = string.IsNullOrWhiteSpace(EditPhone.Text)
                ? selectedRow["DienThoai"].ToString()
                : EditPhone.Text;

            string newEmail = string.IsNullOrWhiteSpace(EditEmail.Text)
                ? selectedRow["Email"].ToString()
                : EditEmail.Text;

            string newCCCD = string.IsNullOrWhiteSpace(EditCCCD.Text)
                ? selectedRow["CCCD"].ToString()
                : EditCCCD.Text;

            string newAddress = string.IsNullOrWhiteSpace(EditAddress.Text)
                ? selectedRow["DiaChi"].ToString()
                : EditAddress.Text;

            string newGender = (EditGender.SelectedItem is ComboBoxItem genderItem && !string.IsNullOrWhiteSpace(genderItem.Content.ToString()))
                ? genderItem.Content.ToString()
                : selectedRow["GioiTinh"].ToString();

            BenhNhan patient = new BenhNhan
            {
                ID_BenhNhan = idBenhNhan,
                HoTenBN = newName,
                NgaySinh = newDOB,
                GioiTinh = newGender,
                CCCD = newCCCD,
                DienThoai = newPhone,
                DiaChi = newAddress,
                Email = newEmail
            };
            bool result = benhnhanBLL.UpdatePatient(patient);
            try
            {
                if (result)
                {
                    MessageBox.Show("Cập nhật thông tin bệnh nhân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    EditPatientPopup.IsOpen = false;
                    LoadDSBenhNhan();
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            EditPatientPopup.IsOpen = false;
        }

        private void btn_ViewExamination_Click(object sender, RoutedEventArgs e)
        {
            var row = dgBenhNhan.SelectedItem as DataRowView;
            string idBenhNhan = row["ID_BenhNhan"].ToString();

            PatientExaminationList form = new PatientExaminationList(idBenhNhan);
            form.RenderTransform = new TranslateTransform();

            var parent = this.Parent as Border;
            if (parent == null) return;

            var slideOut = new DoubleAnimation
            {
                From = 0,
                To = -this.ActualWidth,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var slideIn = new DoubleAnimation
            {
                From = this.ActualWidth,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var currentTransform = new TranslateTransform();
            this.RenderTransform = currentTransform;

            slideOut.Completed += (s, _) =>
            {
                parent.Child = form;

                var newTransform = form.RenderTransform as TranslateTransform;
                newTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
            };

            currentTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
        }

        private void btn_deletePatientFromBenhNhan_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = dgBenhNhan.SelectedItem as DataRowView;

            if (selectedRow == null)
            {
                MessageBox.Show("Vui lòng chọn một bệnh nhân để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa bệnh nhân này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            string idBenhNhan = selectedRow["ID_BenhNhan"].ToString();

            try
            {
                int deleteResult = benhnhanBLL.DeletePatient(Convert.ToInt32(idBenhNhan));

                switch (deleteResult)
                {
                    case 1:
                        MessageBox.Show("Đã xóa bệnh nhân thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDSBenhNhan();
                        break;
                    case 2:
                        MessageBox.Show("Không thể xóa vì bệnh nhân đã có phiếu khám.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 0:
                    default:
                        MessageBox.Show("Không tìm thấy bệnh nhân để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi:\n" + ex.Message, "Lỗi khi xóa", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void ActionMenu_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.DataContext = button.DataContext;
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearchNgayKham.Text))
            {
                txtPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadDSBenhNhan(
                txtSearchByName.Text.Trim(),
                txtSearchByID.Text.Trim(),
                txtSearchLoaiBenh.Text.Trim(),
                txtSearchTrieuChung.Text.Trim(),
                txtSearchNgayKham.Text.Trim()
            );
        }
    }
}
