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

namespace ClinicManagement.InSidebarItems
{
    /// <summary>
    /// Interaction logic for ExaminationFormView.xaml
    /// </summary>
    public partial class ExaminationFormView : UserControl
    {
        private string idBenhNhan;
        private DateTime ngayKham;
        private string connectionString = "Data Source=LAPTOP-2FUIJHRN;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public ExaminationFormView(string idBenhNhan, DateTime ngayKham)
        {
            InitializeComponent();
            this.idBenhNhan = idBenhNhan;
            this.ngayKham = ngayKham;
            lblMaBN.Content = idBenhNhan;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string querry = @"SELECT HoTenBN, PK.TrieuChung, TenLoaiBenh, ID_PhieuKham, TienKham, TongTienThuoc      
                                  FROM PHIEUKHAM PK JOIN BENHNHAN BN ON BN.ID_BenhNhan = PK.ID_BenhNhan JOIN LOAIBENH LB ON PK.ID_LoaiBenh = LB.ID_LoaiBenh
                                  WHERE PK.ID_BenhNhan = @ID_BenhNhan AND PK.NgayKham = @NgayKham
                                ";
                SqlCommand cmd = new SqlCommand(querry, con);
                cmd.Parameters.AddWithValue("@ID_BenhNhan", idBenhNhan);
                cmd.Parameters.AddWithValue("@NgayKham", this.ngayKham);

                SqlDataReader reader = cmd.ExecuteReader();
                string hoTenBN = null;
                string trieuChung = null;
                string tenLoaiBenh = null;
                string idPhieuKham = null;
                decimal tienKham = 0;
                decimal tongTienThuoc = 0;
                while (reader.Read())
                {
                    hoTenBN = reader["HoTenBN"].ToString();
                    trieuChung = reader["TrieuChung"].ToString();
                    tenLoaiBenh = reader["TenLoaiBenh"].ToString();
                    idPhieuKham = reader["ID_PhieuKham"].ToString();
                    tienKham = (decimal)reader["TienKham"];
                    if (reader["TongTienThuoc"] != DBNull.Value)
                        tongTienThuoc = (decimal)reader["TongTienThuoc"];
                    else tongTienThuoc = 0;
                }
                reader.Close();
                if (hoTenBN != null && trieuChung != null && tenLoaiBenh != null && idPhieuKham != null)
                {
                    lblMaPK.Content = idPhieuKham.ToString();
                    lblHoTenBN.Content = hoTenBN.ToString();
                    lblTrieuChung.Content = trieuChung.ToString();
                    lblChuanDoan.Content = tenLoaiBenh.ToString();
                    string querry2 = @"SELECT TenThuoc, TenDVT, SoLuong, MoTaCachDung, DonGiaBan_LucMua, TienThuoc
                                       FROM PHIEUKHAM PK JOIN TOATHUOC CT ON PK.ID_PhieuKham = CT.ID_PhieuKham JOIN THUOC T ON CT.ID_Thuoc = T.ID_Thuoc
                                                        JOIN DVT ON DVT.ID_DVT = T.ID_DVT JOIN CACHDUNG C ON C.ID_CachDung = T.ID_CachDUng
                                       WHERE PK.ID_PhieuKham = @ID_PhieuKham";
                    SqlDataAdapter adapter = new SqlDataAdapter(querry2, con);
                    adapter.SelectCommand.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);
                    DataTable dtThuoc = new DataTable();
                    adapter.Fill(dtThuoc);

                    dgToaThuoc.ItemsSource = dtThuoc.DefaultView;
                    lblTienKham.Content = tienKham.ToString("N0");
                    lblTongTienThuoc.Content = tongTienThuoc.ToString("N0");
                    lblNgayKham.Content = ngayKham.ToString();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin bệnh nhân.");
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
    }
    
}
