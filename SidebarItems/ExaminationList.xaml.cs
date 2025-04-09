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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClinicManagement.SidebarItems
{
    /// <summary>
    /// Interaction logic for ExaminationList.xaml
    /// </summary>
    public partial class ExaminationList : UserControl
    {
        private DateTime thoiDiem;
        public ExaminationList()
        {
            InitializeComponent();
            thoiDiem = DateTime.Now;
            dpNgayKham.SelectedDate = thoiDiem.Date;   // Chọn ngày hôm nay trên DatePicker
            LoadDSTiepNhan(thoiDiem.Date);
        }

        private void LoadDSTiepNhan(DateTime NgayKham)
        {
            string connectionString = "Data Source=LAPTOP-2FUIJHRN;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            string querry = @"
                SELECT 
                    BN.ID_BenhNhan, BN.HoTenBN, BN.NgaySinh, BN.GioiTinh, TN.ThoiGianTiepNhan, TN.ID_NhanVien
                FROM DANHSACHTIEPNHAN TN JOIN BENHNHAN BN ON TN.ID_BenhNhan = BN.ID_BenhNhan
                WHERE TN.Is_Deleted = 0 AND CAST (TN.ThoiGianTiepNhan AS DATE) = @NgayKham
                ORDER BY TN.ThoiGianTiepNhan asc
            ";
            string QDquerry = "SELECT GiaTri FROM QUI_DINH WHERE TenQuiDinh = 'SoLuongPhieuKhamToiDa'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(querry, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@NgayKham", NgayKham);
                    SqlCommand cmd = new SqlCommand(QDquerry, con);
                    var SLBNMax = cmd.ExecuteScalar();
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dt.Columns.Add("STT", typeof(int));
                    for (int i =0; i<dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["STT"] = i + 1;
                    }

                    lblRowCount.Content = dt.Rows.Count.ToString();
                    lblMax.Content = SLBNMax;
                    dgTiepNhan.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi" + ex.Message);
                }
            }    
        }

        private void btn_addPatientToExam_Click(object sender, RoutedEventArgs e)
        {
            thoiDiem = DateTime.Now;
            lblNgayHienTai.Content =  thoiDiem.ToString("dd/MM/yyyy");
            AddPatientPopup.IsOpen = true;

            tbMaBN.Text = "";
            tbMaNV.Text = "";
            AddPatientPopup.HorizontalOffset = 500;
            AddPatientPopup.VerticalOffset = 300;
        }

        private void btn_editPatientFromExam_Click(object sender, RoutedEventArgs e)
        {
            thoiDiem = DateTime.Now;
            lblEditNgayHienTai.Content = thoiDiem.ToString("dd/MM/yyyy");

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
            string connectionString = "Data Source=LAPTOP-2FUIJHRN;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
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

        }

        private void btn_viewExamForm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dpNgayKham_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpNgayKham.SelectedDate.HasValue)
            {
                DateTime ngayChon = dpNgayKham.SelectedDate.Value;
                LoadDSTiepNhan(ngayChon);
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

            if (string.IsNullOrEmpty(maBN) || string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bệnh nhân và nhân viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string connectionString = "Data Source=LAPTOP-2FUIJHRN;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "INSERT INTO DANHSACHTIEPNHAN (ID_BenhNhan, ID_NhanVien, ThoiGianTiepNhan) VALUES (@ID_BenhNhan, @ID_NhanVien, @ThoiGianTiepNhan)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ID_BenhNhan", maBN);
                    cmd.Parameters.AddWithValue("@ID_NhanVien", maNV);
                    cmd.Parameters.AddWithValue("@ThoiGianTiepNhan", thoiDiem);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Thêm bệnh nhân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        if (dpNgayKham.SelectedDate.HasValue)
                            LoadDSTiepNhan(dpNgayKham.SelectedDate.Value);
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

            string connectionString = "Data Source=LAPTOP-2FUIJHRN;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
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

                        if (dpNgayKham.SelectedDate.HasValue)
                            LoadDSTiepNhan(dpNgayKham.SelectedDate.Value);
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

        }
    }
}
