using ClinicManagement.SidebarItems;
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

namespace ClinicManagement.InSidebarItems
{
    public partial class ExaminationFormList : UserControl
    {
        private string connectionString = "Data Source=LAPTOP-5EKP9JC6\\SQLEXPRESS01;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private string ID_BenhNhan;
        public ExaminationFormList(string ID_BenhNhan)
        {
            InitializeComponent();
            this.ID_BenhNhan = ID_BenhNhan;
            DataContext = this;
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

        public class Patient
        {
            public int ID { get; set; }
            public string HoTen { get; set; }
            public string NgaySinh { get; set; }
            public string GioiTinh { get; set; }
        }

        private Patient GetPatientById(string id)
        {
            Patient patient = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM BENHNHAN WHERE ID_BenhNhan = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        patient = new Patient
                        {
                            ID = Convert.ToInt32(reader["ID_BenhNhan"]),
                            HoTen = reader["HoTenBN"].ToString(),
                            NgaySinh = Convert.ToDateTime(reader["NgaySinh"]).ToString("dd/MM/yyyy"),
                            GioiTinh = reader["GioiTinh"].ToString(),
                        };
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi truy vấn CSDL: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return patient;
        }
        private void LoadPatientInfo(string id)
        {
            Patient patient = GetPatientById(id);

            if (patient != null)
            {
                txtIDPatienr.Text = patient.ID.ToString();
                txtPatientName.Text = patient.HoTen;
                txtPatientSex.Text = patient.GioiTinh;
                txtPatientDate.Text = patient.NgaySinh;

                // Hiển thị icon giới tính nếu bạn đã thêm phần đó trong XAML
                if (patient.GioiTinh.ToLower() == "nam")
                {
                    iconGender.Kind = MaterialDesignThemes.Wpf.PackIconKind.FaceMan;
                }
                else if (patient.GioiTinh.ToLower() == "nữ")
                {
                    iconGender.Kind = MaterialDesignThemes.Wpf.PackIconKind.FaceWoman;
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin bệnh nhân.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public class PhieuKham
        {
            public int ID_PhieuKham { get; set; }
            public DateTime? NgayKham { get; set; }  
            public string TienKham { get; set; }   
            public string TongTienThuoc { get; set; } 
        }
        private void LoadPhieuKham()
        {
            List<PhieuKham> danhSachPhieu = new List<PhieuKham>();
            string query = @"SELECT ID_PhieuKham, NgayKham, TienKham, TongTienThuoc 
                             FROM PHIEUKHAM 
                             WHERE ID_BenhNhan = @ID_BenhNhan";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ID_BenhNhan", ID_BenhNhan);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        danhSachPhieu.Add(new PhieuKham
                        {
                            ID_PhieuKham = reader.GetInt32(0),
                            NgayKham = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1),  // Nếu NULL thì gán là null
                            TienKham = reader.IsDBNull(2) ? "0" : reader.GetDecimal(2).ToString("N0"),  // Nếu NULL thì gán "Chưa có"
                            TongTienThuoc = reader.IsDBNull(3) ? "0" : reader.GetDecimal(3).ToString("N0")  // Nếu NULL thì gán "Chưa có"
                        });
                    }
                }
            }

            dgPhieuKham.ItemsSource = danhSachPhieu;
            txtExaminationCount.Text = danhSachPhieu.Count.ToString();
        }



        private void btn_ViewExamination_Click(object sender, RoutedEventArgs e)
        {
            var selectedPhieu = ((FrameworkElement)sender).DataContext as PhieuKham;
            if (selectedPhieu == null)
                return;

            int idPhieuKham = selectedPhieu.ID_PhieuKham;
            ShowExaminationPopup(idPhieuKham);
        }

        public class Thuoc
        {
            public int ID_PhieuKham { get; set; }
            public string TenThuoc { get; set; }
            public int SoLuong { get; set; }
            public string TienThuoc { get; set; }  // dạng string để format tiền tệ
            public string MoTaCachDung { get; set; }
        }

        private void LoadDanhSachThuoc(int idPhieuKham, string query)
        {
            List<Thuoc> danhSachThuoc = new List<Thuoc>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        danhSachThuoc.Add(new Thuoc
                        {
                            ID_PhieuKham = reader.GetInt32(reader.GetOrdinal("ID_PhieuKham")),
                            TenThuoc = reader["TenThuoc"].ToString(),
                            SoLuong = reader.GetInt32(reader.GetOrdinal("SoLuong")),
                            TienThuoc = reader.IsDBNull(reader.GetOrdinal("TienThuoc"))
                                ? "Chưa có"
                                : Convert.ToDecimal(reader["TienThuoc"]).ToString("N0"),
                            MoTaCachDung = reader["MoTaCachDung"].ToString()
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách thuốc: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            dgDSThuoc.ItemsSource = danhSachThuoc;
        }

        private void ShowExaminationPopup(int idPhieuKham)
        {
            string querry = @"SELECT HoTenBN, PK.TrieuChung, TenLoaiBenh, ID_PhieuKham, TienKham, TongTienThuoc, PK.NgayKham      
                                  FROM PHIEUKHAM PK JOIN BENHNHAN BN ON BN.ID_BenhNhan = PK.ID_BenhNhan JOIN LOAIBENH LB ON PK.ID_LoaiBenh = LB.ID_LoaiBenh
                                  WHERE PK.ID_PhieuKham = @ID_PhieuKham
                                ";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(querry, con);
                    cmd.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);

                    SqlDataReader reader = cmd.ExecuteReader();
                    string hoTenBN = null;
                    string trieuChung = null;
                    string tenLoaiBenh = null;
                    decimal tienKham = 0;
                    decimal tongTienThuoc = 0;
                    DateTime ngayKham = DateTime.Now;
                    while (reader.Read())
                    {
                        hoTenBN = reader["HoTenBN"].ToString();
                        trieuChung = reader["TrieuChung"].ToString();
                        tenLoaiBenh = reader["TenLoaiBenh"].ToString();
                        ngayKham = (DateTime) reader["NgayKham"];
                        tienKham = (decimal)reader["TienKham"];
                        if (reader["TongTienThuoc"] != DBNull.Value)
                            tongTienThuoc = (decimal)reader["TongTienThuoc"];
                        else tongTienThuoc = 0;
                    }
                    reader.Close();

                    if (hoTenBN != null && trieuChung != null && tenLoaiBenh != null)
                    {
                        txtIDPhieuKham.Text = idPhieuKham.ToString();
                        txtNgayKham.Text = ngayKham.ToString();
                        txtTrieuChung.Text = trieuChung.ToString();
                        txtTenLoaiBenh.Text = tenLoaiBenh.ToString();
                        txtTienKham.Text = tienKham.ToString("N0");
                        txtTongTienThuoc.Text = tongTienThuoc.ToString("N0");

                        ExaminationPopup.IsOpen = true;

                        string querry2 = @"SELECT TenThuoc, TenDVT, SoLuong, MoTaCachDung, DonGiaBan_LucMua, TienThuoc
                                       FROM PHIEUKHAM PK JOIN TOATHUOC CT ON PK.ID_PhieuKham = CT.ID_PhieuKham JOIN THUOC T ON CT.ID_Thuoc = T.ID_Thuoc
                                                        JOIN DVT ON DVT.ID_DVT = T.ID_DVT JOIN CACHDUNG C ON C.ID_CachDung = T.ID_CachDUng
                                       WHERE PK.ID_PhieuKham = @ID_PhieuKham";
                        SqlDataAdapter adapter = new SqlDataAdapter(querry2, con);
                        adapter.SelectCommand.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);
                        DataTable dtThuoc = new DataTable();
                        adapter.Fill(dtThuoc);

                        dgDSThuoc.ItemsSource = dtThuoc.DefaultView;                        
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải chi tiết phiếu khám: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            // Tạo control cũ (PatientList)
            PatientList previous = new PatientList();
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
