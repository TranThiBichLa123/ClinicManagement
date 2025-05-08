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
using ClinicManagement.InSidebarItems;
using System.Windows.Media.Animation;

namespace ClinicManagement.SidebarItems
{
    public partial class PatientList : UserControl
    {
        string connectionString = "Data Source=LAPTOP-5EKP9JC6\\SQLEXPRESS01;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public PatientList()
        {
            InitializeComponent();
            LoadDSBenhNhan();
        }

        private void LoadDSBenhNhan(string nameKeyword = "", string idKeyword = "", string loaiBenhKeyword = "", string trieuChungKeyword = "", string ngayDK = "")
        {
            string query = @"
                        SELECT 
                            BN.ID_BenhNhan,
                            BN.HoTenBN,
                            BN.NgaySinh,
                            BN.GioiTinh,
                            BN.CCCD,
                            BN.DienThoai,
                            BN.DiaChi,
                            BN.Email,
                            BN.NgayDK
                        FROM BENHNHAN BN
                        WHERE BN.Is_Deleted = 0
";

            if (!string.IsNullOrEmpty(loaiBenhKeyword) || !string.IsNullOrEmpty(trieuChungKeyword))
            {
                query += @"
                        AND EXISTS (
                            SELECT 1
                            FROM PHIEUKHAM PK
                            JOIN DANHSACHTIEPNHAN TN ON TN.ID_TiepNhan=PK.ID_TiepNhan
                            JOIN LOAIBENH LB ON PK.ID_LoaiBenh = LB.ID_LoaiBenh
                            WHERE TN.ID_BenhNhan = BN.ID_BenhNhan
    ";

                if (!string.IsNullOrEmpty(loaiBenhKeyword))
                    query += " AND LB.TenLoaiBenh LIKE @loaiBenhKeyword";

                if (!string.IsNullOrEmpty(trieuChungKeyword))
                    query += " AND PK.TrieuChung LIKE @trieuChungKeyword";

                query += ")";
            }

            if (!string.IsNullOrEmpty(nameKeyword))
                query += " AND BN.HoTenBN LIKE @nameKeyword";
            if (!string.IsNullOrEmpty(idKeyword))
                query += " AND CAST(BN.ID_BenhNhan AS NVARCHAR) LIKE @idKeyword";
            if (!string.IsNullOrEmpty(ngayDK))
                query += " AND CONVERT(VARCHAR, BN.NgayDK, 103) LIKE @ngayDK";


            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);

                    if (!string.IsNullOrEmpty(nameKeyword))
                        cmd.Parameters.AddWithValue("@nameKeyword", "%" + nameKeyword + "%");
                    if (!string.IsNullOrEmpty(idKeyword))
                        cmd.Parameters.AddWithValue("@idKeyword", "%" + idKeyword + "%");
                    if (!string.IsNullOrEmpty(loaiBenhKeyword))
                        cmd.Parameters.AddWithValue("@loaiBenhKeyword", "%" + loaiBenhKeyword + "%");
                    if (!string.IsNullOrEmpty(trieuChungKeyword))
                        cmd.Parameters.AddWithValue("@trieuChungKeyword", "%" + trieuChungKeyword + "%");
                    if (!string.IsNullOrEmpty(ngayDK))
                        cmd.Parameters.AddWithValue("@ngayDK", "%" + ngayDK + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (row[col] == DBNull.Value)
                            {
                                row[col] = "0";
                            }
                        }
                    }
                    dgBenhNhan.ItemsSource = dt.DefaultView;
                    txtPatientCount.Text = dt.Rows.Count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải danh sách bệnh nhân: " + ex.Message);
                }
            }
        }

        private void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            AddPatientPopup.IsOpen = true;
        }

        public class BenhNhan
        {
            public string HoTenBN { get; set; }
            public DateTime NgaySinh { get; set; }
            public string GioiTinh { get; set; }
            public string CCCD { get; set; }
            public string DienThoai { get; set; }
            public string DiaChi { get; set; }
            public string Email { get; set; }

            public DateTime NgayDK { get; set; }
            public bool Is_Deleted { get; set; }
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
                bool isAdded = AddPatientToDatabase(newPatient);

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

        private bool AddPatientToDatabase(BenhNhan patient)
        {
            string query = @"
                INSERT INTO BenhNhan (HoTenBN, NgaySinh, GioiTinh, CCCD, DienThoai, DiaChi, Email, NgayDK, Is_Deleted)
                VALUES (@HoTenBN, @NgaySinh, @GioiTinh, @CCCD, @DienThoai, @DiaChi, @Email, @NgayDK, @Is_Deleted)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@HoTenBN", patient.HoTenBN);
                cmd.Parameters.AddWithValue("@NgaySinh", patient.NgaySinh);
                cmd.Parameters.AddWithValue("@GioiTinh", patient.GioiTinh);
                cmd.Parameters.AddWithValue("@CCCD", patient.CCCD);
                cmd.Parameters.AddWithValue("@DienThoai", patient.DienThoai);
                cmd.Parameters.AddWithValue("@DiaChi", patient.DiaChi);
                cmd.Parameters.AddWithValue("@Email", patient.Email);
                cmd.Parameters.AddWithValue("@NgayDK", patient.NgayDK);
                cmd.Parameters.AddWithValue("@Is_Deleted", patient.Is_Deleted);

                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
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

            // Đổ dữ liệu cũ vào popup
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

            string newGender;
            if (EditGender.SelectedItem is ComboBoxItem genderItem && !string.IsNullOrWhiteSpace(genderItem.Content.ToString()))
            {
                newGender = genderItem.Content.ToString(); 
            }
            else
            {
                newGender = selectedRow["GioiTinh"].ToString();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                        UPDATE BENHNHAN
                        SET HoTenBN = @HoTenBN,
                            NgaySinh = @NgaySinh,
                            GioiTinh = @GioiTinh,
                            CCCD = @CCCD,
                            DienThoai = @DienThoai,
                            DiaChi = @DiaChi,
                            Email = @Email
                        WHERE ID_BenhNhan = @ID_BenhNhan";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HoTenBN", newName);
                cmd.Parameters.AddWithValue("@NgaySinh", newDOB);
                cmd.Parameters.AddWithValue("@GioiTinh", newGender);
                cmd.Parameters.AddWithValue("@CCCD", newCCCD);
                cmd.Parameters.AddWithValue("@DienThoai", newPhone);
                cmd.Parameters.AddWithValue("@DiaChi", newAddress);
                cmd.Parameters.AddWithValue("@Email", newEmail);
                cmd.Parameters.AddWithValue("@ID_BenhNhan", idBenhNhan);

                try
                {
                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
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

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string querry = "UPDATE BENHNHAN SET Is_Deleted = 1 WHERE ID_BenhNhan = @ID_BenhNhan";

                SqlCommand cmd = new SqlCommand(querry, conn);
                cmd.Parameters.AddWithValue("@ID_BenhNhan", idBenhNhan);
                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Đã xóa bệnh nhân khỏi danh sách tiếp nhận.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadDSBenhNhan();
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
