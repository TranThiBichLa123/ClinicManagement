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

namespace ClinicManagement.InSidebarItems
{
    /// <summary>
    /// Interaction logic for ExaminationFormView.xaml
    /// </summary>
    public partial class ExaminationFormView : UserControl
    {
        private string idBenhNhan;
        private int Thang;
        private int Nam;
        private int idTiepNhan;
        private int idPK;
        private string connectionString = "Data Source=LAPTOP-2FUIJHRN;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public ExaminationFormView(string idBenhNhan, int idTiepNhan)
        {
            InitializeComponent();
            this.idBenhNhan = idBenhNhan;
            this.idTiepNhan = idTiepNhan;
            lblMaBN.Content = idBenhNhan;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                DateTime ngayTN = DateTime.MinValue;
                con.Open();
                string querry = "SELECT NgayTN FROM DANHSACHTIEPNHAN WHERE ID_TiepNhan = @id_TiepNhan";
                SqlCommand cmd = new SqlCommand(querry, con);
                cmd.Parameters.AddWithValue("@id_TiepNhan", idTiepNhan); // Corrected parameter name
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ngayTN = (DateTime)reader["NgayTN"];
                }
                reader.Close();
                Thang = ngayTN.Month;
                Nam = ngayTN.Year;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string querry = @"SELECT HoTenBN, PK.TrieuChung, TenLoaiBenh, ID_PhieuKham, TienKham, TongTienThuoc, CaKham, NgayTN     
                                  FROM PHIEUKHAM PK JOIN DANHSACHTIEPNHAN TN ON PK.ID_TiepNhan = TN.ID_TiepNhan
                                                    JOIN BENHNHAN BN ON BN.ID_BenhNhan = TN.ID_BenhNhan 
                                                    JOIN LOAIBENH LB ON PK.ID_LoaiBenh = LB.ID_LoaiBenh
                                  WHERE PK.ID_TiepNhan = @ID_TiepNhan
                                ";
                SqlCommand cmd = new SqlCommand(querry, con);
                cmd.Parameters.AddWithValue("@ID_TiepNhan", this.idTiepNhan);

                SqlDataReader reader = cmd.ExecuteReader();
                string hoTenBN = null;
                string trieuChung = null;
                string tenLoaiBenh = null;
                string idPhieuKham = null;
                string caKham = null;
                string ngayTN = null;
                decimal tienKham = 0;
                decimal tongTienThuoc = 0;
                while (reader.Read())
                {
                    hoTenBN = reader["HoTenBN"].ToString();
                    trieuChung = reader["TrieuChung"].ToString();
                    tenLoaiBenh = reader["TenLoaiBenh"].ToString();
                    idPhieuKham = reader["ID_PhieuKham"].ToString();
                    idPK = Convert.ToInt32(idPhieuKham);
                    caKham = reader["CaKham"].ToString();
                    ngayTN = ((DateTime)reader["NgayTN"]).ToString("dd/MM/yyyy");
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
                    lblNgayKham.Content = "Ca " + caKham + " Ngày " + ngayTN;
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

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string querry = @"UPDATE PHIEUKHAM SET Is_Deleted = 1 WHERE ID_PhieuKham = @ID_PhieuKham";
                string querry2 = @"DELETE FROM TOATHUOC WHERE ID_PhieuKham = @ID_PhieuKham";
                try
                {
                    conn.Open();
                    SqlCommand cmd2 = new SqlCommand(querry2, conn);
                    cmd2.Parameters.AddWithValue("@ID_PhieuKham", this.idPK);
                    cmd2.ExecuteNonQuery();

                    SqlCommand cmd = new SqlCommand(querry, conn);
                    cmd.Parameters.AddWithValue("@ID_PhieuKham", this.idPK);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Đã xóa phiếu khám và các toa thuốc tương ứng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        //Tạo hiệu ứng slide-out sang phải cho form hiện tại

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

        private void btnSuaPK_Click(object sender, RoutedEventArgs e)
        {
            int idPK = this.idPK;
            string idBN = this.idBenhNhan;
            int idTN = this.idTiepNhan;
            // → Ở đây bạn có thể truyền ID vào ExaminationForm
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