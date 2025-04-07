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
        public ExaminationList()
        {
            InitializeComponent();
        }

        private void LoadDSTiepNhan(DateTime NgayKham)
        {
            string connectionString = "Data Source=LAPTOP-2FUIJHRN;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            string querry = @"
                SELECT 
                    BN.ID_BenhNhan, BN.HoTenBN, BN.NgaySinh, BN.GioiTinh, BN.CCCD
                FROM DANHSACHTIEPNHAN TN JOIN BENHNHAN BN ON TN.ID_BenhNhan = BN.ID_BenhNhan
                WHERE CAST (TN.ThoiGianTiepNhan AS DATE) = @NgayKham
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
            AddPatientPopup.IsOpen = true;
        }

        private void btn_editPatientFromExam_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_deletePatientFromExam_Click(object sender, RoutedEventArgs e)
        {

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

        private void AddPatientPopup_Click(object sender, RoutedEventArgs e)
        {
            // Lấy mã bệnh nhân từ TextBox
            string patientID = txtPatientID.Text;

            // Thực hiện thêm bệnh nhân vào cơ sở dữ liệu hoặc danh sách (nếu có)

            MessageBox.Show("Bệnh nhân đã được thêm: " + patientID);

            // Đóng Popup
            AddPatientPopup.IsOpen = false;
        }
    }
}
