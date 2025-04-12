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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Windows.Media.Animation;

namespace ClinicManagement.InSidebarItems
{
    /// <summary>
    /// Interaction logic for ExaminationForm.xaml
    /// </summary>
    


    public partial class ExaminationForm : UserControl
    {
        public class ThuocDaChon
        {
            public string ID_Thuoc { get; set; }
            public decimal DonGiaBan { get; set; }
            public string TenThuoc { get; set; }
            public int SoLuong { get; set; } = 1;
        }

        private string idBenhNhan;
        private DateTime thoiGianTiepNhan;
        private string connectionString = "Data Source=LAPTOP-2FUIJHRN;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private ObservableCollection<ThuocDaChon> danhSachThuoc = new ObservableCollection<ThuocDaChon>();

        public ExaminationForm(string idBenhNhan, DateTime thoiGianTiepNhan)
        {
            InitializeComponent();
            this.idBenhNhan = idBenhNhan;
            this.thoiGianTiepNhan = thoiGianTiepNhan;
            txtMaBenhNhan.Text = idBenhNhan;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string querry = "SELECT HoTenBN from BENHNHAN WHERE ID_BenhNhan = @ID_BenhNhan";
                SqlCommand cmd = new SqlCommand(querry, con);
                cmd.Parameters.AddWithValue("@ID_BenhNhan", idBenhNhan);
                var tenBN = cmd.ExecuteScalar();
                if (tenBN != null)
                {
                    txtTenBenhNhan.Text = tenBN.ToString();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin bệnh nhân.");
                }
            }

            LoadLoaiBenh();
            dgThuocDaChon.ItemsSource = danhSachThuoc;
            LoadThuoc();
        }
        private void LoadLoaiBenh()
        {
            string query = "SELECT ID_LoaiBenh, TenLoaiBenh FROM LOAIBENH";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cboLoaiBenh.ItemsSource = dt.DefaultView;
                cboLoaiBenh.DisplayMemberPath = "TenLoaiBenh";
                cboLoaiBenh.SelectedValuePath = "ID_LoaiBenh";
            }
        }

        private void LoadThuoc()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string querry = @"SELECT T.ID_Thuoc, T.TenThuoc, T.SoLuongTon, T.DonGiaBan, C.MoTaCachDung, T.ID_DVT, D.TenDVT, T.HinhAnh
                                    FROM THUOC T JOIN DVT D ON T.ID_DVT = D.ID_DVT JOIN CACHDUNG C ON C.ID_CachDung = T.ID_CachDung";

                SqlDataAdapter adapter = new SqlDataAdapter(querry, connectionString);
                DataTable dtThuoc = new DataTable();
                adapter.Fill(dtThuoc);

                // Đổ vào ComboBox
                cboThuoc.ItemsSource = dtThuoc.DefaultView;
                cboThuoc.DisplayMemberPath = "TenThuoc";
                cboThuoc.SelectedValuePath = "ID_Thuoc";
            }
        }

        private void btnThemThuoc_Click(object sender, RoutedEventArgs e)
        {
            if (cboThuoc.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn thuốc trước khi thêm.");
                return;
            }

            if (!int.TryParse(txtSoLuong.Text.Trim(), out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng hợp lệ (> 0).");
                return;
            }

            var selectedThuoc = cboThuoc.SelectedItem as DataRowView;
            string id = selectedThuoc["ID_Thuoc"].ToString();
            string ten = selectedThuoc["TenThuoc"].ToString();
            decimal donGiaBan = Convert.ToDecimal(selectedThuoc["DonGiaBan"]);
            int soLuongTon = Convert.ToInt32(selectedThuoc["SoLuongTon"]);

            if (soLuong > soLuongTon)
            {
                MessageBox.Show($"Số lượng vượt quá tồn kho ({soLuongTon}).");
                return;
            }

            var daTonTai = danhSachThuoc.FirstOrDefault(t => t.ID_Thuoc == id);
            if (daTonTai != null)
            {
                if (daTonTai.SoLuong + soLuong > soLuongTon)
                {
                    MessageBox.Show($"Tổng số lượng vượt tồn kho ({soLuongTon}).");
                    return;
                }

                daTonTai.SoLuong += soLuong;
            }
            else
            {
                danhSachThuoc.Add(new ThuocDaChon
                {
                    ID_Thuoc = id,
                    TenThuoc = ten,
                    DonGiaBan = donGiaBan,
                    SoLuong = soLuong
                });
            }

            dgThuocDaChon.Items.Refresh();
            txtSoLuong.Text = "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DateTime thoiGianKham = DateTime.Now;
            string query = "INSERT INTO PHIEUKHAM (ID_BenhNhan, ThoiGianTiepNhan, TrieuChung, ID_LoaiBenh, NgayKham) VALUES (@ID_BenhNhan, @ThoiGianTiepNhan, @TrieuChung, @ID_LoaiBenh, @NgayKham); SELECT SCOPE_IDENTITY();";
            int idPhieuKham;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID_BenhNhan", idBenhNhan);
                cmd.Parameters.AddWithValue("@NgayKham", thoiGianKham);
                cmd.Parameters.Add("@ThoiGianTiepNhan", SqlDbType.DateTime).Value = thoiGianTiepNhan;
                cmd.Parameters.AddWithValue("@TrieuChung", txtTrieuChung.Text.Trim());
                cmd.Parameters.AddWithValue("@ID_LoaiBenh", cboLoaiBenh.SelectedValue);
                // Thực thi lệnh và lấy ID vừa tạo
                object result = cmd.ExecuteScalar();
            }



            //Lấy ID_PhieuKham vừa tạo
            string getIDquerry = "SELECT ID_PhieuKham FROM PHIEUKHAM WHERE ID_BenhNhan = @idBenhNhan AND NgayKham = @ngayKham";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(getIDquerry, con);
                cmd.Parameters.AddWithValue("@idBenhNhan", idBenhNhan);
                cmd.Parameters.AddWithValue("@ngayKham", thoiGianKham);
                idPhieuKham = (int)cmd.ExecuteScalar();
            }    


            // Lưu thông tin thuốc vào bảng TOATHUOC
            foreach (var thuoc in danhSachThuoc)
            {
                string queryInsert = "INSERT INTO TOATHUOC (ID_PhieuKham, ID_Thuoc, DonGiaBan_LucMua, SoLuong) VALUES (@ID_PhieuKham, @ID_Thuoc, @donGiaBan, @SoLuong); ";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(queryInsert, con);
                    cmd.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);
                    cmd.Parameters.AddWithValue("@ID_Thuoc", thuoc.ID_Thuoc);
                    cmd.Parameters.AddWithValue("@donGiaBan", thuoc.DonGiaBan); 
                    cmd.Parameters.AddWithValue("@SoLuong", thuoc.SoLuong);
                    cmd.ExecuteNonQuery();
                }
            }

            // Gọi stored procedure cập nhật báo cáo sử dụng thuốc (nếu muốn)
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_CapNhatBaoCaoSuDungThuoc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Lưu thành công thông tin khám bệnh và toa thuốc.");

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

        private void cboThuoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (cboThuoc.SelectedItem is DataRowView selectedThuoc)
            {
                string duongDanAnh = "";
                txtTenThuoc.Text = selectedThuoc["TenThuoc"].ToString() + ":     ";
                txtSoLuongTon.Text = selectedThuoc["SoLuongTon"].ToString();
                duongDanAnh = selectedThuoc["HinhAnh"].ToString();
                try
                {
                    var imageUri = new Uri(duongDanAnh, UriKind.RelativeOrAbsolute);
                    var imageBrush = new ImageBrush(new BitmapImage(imageUri));
                    imageBrush.Stretch = Stretch.UniformToFill;
                    thuocImageBorder.Background = imageBrush;
                }
                catch
                {
                    // Nếu lỗi thì đặt nền mặc định
                    thuocImageBorder.Background = Brushes.LightBlue;
                }

                txtSoLuong.Focus();
                txtSoLuong.SelectAll();
            }
            
        }

        private void btnXoaThuoc_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dòng đang chọn trong DataGrid
            var selectedRow = dgThuocDaChon.SelectedItem as ThuocDaChon;

            if (selectedRow == null)
            {
                MessageBox.Show("Vui lòng chọn một loại thuốc để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Xác nhận trước khi xóa
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa loại thuốc này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;
            danhSachThuoc.Remove(selectedRow);

        }

        private void btnSuaThuoc_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = dgThuocDaChon.SelectedItem as ThuocDaChon;

            if (selectedRow == null)
            {
                MessageBox.Show("Vui lòng chọn một loại thuốc để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Xác nhận trước khi xóa
            var result = MessageBox.Show("Bạn có chắc chắn muốn sửa loại thuốc này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            string soLuongEdited = txtSoLuong.Text.Trim();
            if (string.IsNullOrEmpty(soLuongEdited))
            {
                MessageBox.Show("Vui lòng nhập số lượng.");
                return;
            }
            if (!int.TryParse(txtSoLuong.Text.Trim(), out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng hợp lệ (> 0).");
                return;
            }
            else selectedRow.SoLuong = Convert.ToInt32(soLuongEdited);
            // Cập nhật lại giao diện DataGrid
            dgThuocDaChon.Items.Refresh();
            txtSoLuong.Text = "";
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
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

        private void dgThuocDaChon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = dgThuocDaChon.SelectedItem as ThuocDaChon;
            if (selected == null) return;

            // Lấy danh sách thuốc đang load trong ComboBox
            var dtThuoc = cboThuoc.ItemsSource as DataView;
            if (dtThuoc == null) return;

            // Tìm dòng DataRowView tương ứng với ID_Thuoc đang chọn
            foreach (DataRowView row in dtThuoc)
            {
                if (row["ID_Thuoc"].ToString() == selected.ID_Thuoc)
                {
                    cboThuoc.SelectedItem = row;
                    break;
                }
            }

            // Set lại số lượng vào ô textbox nếu muốn
            txtSoLuong.Text = selected.SoLuong.ToString();

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
    }
}
