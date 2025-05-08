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
  


    public partial class ExaminationForm : UserControl
    {
        public class ThuocDaChon
        {
            public string ID_Thuoc { get; set; }
            public decimal DonGiaBan { get; set; }
            public string MoTa { get; set; }
            public string DonViTinh { get; set; }
            public string TenThuoc { get; set; }
            public int SoLuong { get; set; } = 1;
        }

        private int idTiepNhan;
        private int Thang;
        private int Nam;
        private string idBenhNhan;
        private int? idPhieuKham = null;
        private string connectionString = "Data Source=LAPTOP-2FUIJHRN;Initial Catalog=QL_PHONGMACHTU;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private ObservableCollection<ThuocDaChon> danhSachThuoc = new ObservableCollection<ThuocDaChon>();

        public ExaminationForm(string idBenhNhan, int idTiepNhan)
        {
            InitializeComponent();
            tblTitle.Text = "Tạo Phiếu Khám";
            this.idBenhNhan = idBenhNhan;
            this.idTiepNhan = idTiepNhan;
            txtMaBenhNhan.Text = idBenhNhan;


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

        public ExaminationForm(string idBN, int idTN, int idPK)
        {
            InitializeComponent();
            tblTitle.Text = "Sửa Phiếu Khám";
            this.idBenhNhan = idBN;

            this.idTiepNhan= idTN;
            this.idPhieuKham = idPK;

            LoadLoaiBenh();
            
            LoadPhieuKham(idPK);

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

        private void LoadPhieuKham(int idPhieuKham)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT TN.ID_BenhNhan, HoTenBN, TrieuChung, ID_LoaiBenh, CaKham, NgayTN 
                                    FROM PHIEUKHAM PK JOIN DANHSACHTIEPNHAN TN ON PK.ID_TiepNhan = TN.ID_TiepNhan
                                                      JOIN BENHNHAN BN ON BN.ID_BenhNhan = TN.ID_BenhNhan
                                    WHERE ID_PhieuKham = @ID_PhieuKham AND PK.Is_Deleted = 0";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtTrieuChung.Text = reader["TrieuChung"].ToString();
                    txtCaKham.Text = reader["CaKham"].ToString();
                    txtMaBenhNhan.Text = idBenhNhan.ToString();
                    txtTenBenhNhan.Text = reader["HoTenBN"].ToString();
                    cboLoaiBenh.SelectedValue = reader["ID_LoaiBenh"];
                }
                reader.Close();

                // Load danh sách thuốc trong toa thuốc và chuyển sang ObservableCollection
                string querry2 = @"SELECT CT.ID_Thuoc, TenThuoc, TenDVT, SoLuong, MoTaCachDung, DonGiaBan_LucMua, TienThuoc
                                       FROM PHIEUKHAM PK JOIN TOATHUOC CT ON PK.ID_PhieuKham = CT.ID_PhieuKham JOIN THUOC T ON CT.ID_Thuoc = T.ID_Thuoc
                                                        JOIN DVT ON DVT.ID_DVT = T.ID_DVT JOIN CACHDUNG C ON C.ID_CachDung = T.ID_CachDUng
                                       WHERE PK.ID_PhieuKham = @ID_PhieuKham";
                SqlDataAdapter adapter = new SqlDataAdapter(querry2, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // ======= Sửa đoạn này để chuyển DataTable thành ObservableCollection =======
                danhSachThuoc.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    danhSachThuoc.Add(new ThuocDaChon
                    {
                        ID_Thuoc = row["ID_Thuoc"].ToString(),
                        TenThuoc = row["TenThuoc"].ToString(),
                        DonViTinh = row["TenDVT"].ToString(),
                        MoTa = row["MoTaCachDung"].ToString(),
                        DonGiaBan = Convert.ToDecimal(row["DonGiaBan_LucMua"]),
                        SoLuong = Convert.ToInt32(row["SoLuong"])
                    });
                }
                dgThuocDaChon.ItemsSource = danhSachThuoc;
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
            string querryAddThuoc = @"SELECT TenDVT, MoTaCachDung, TienThuoc FROM
                                            DVT JOIN THUOC ON THUOC.ID_DVT = DVT.ID_DVT
                                                JOIN TOATHUOC ON TOATHUOC.ID_Thuoc = THUOC.ID_Thuoc
                                                JOIN CACHDUNG ON CACHDUNG.ID_CachDung = THUOC.ID_CachDung
                                            WHERE TOATHUOC.ID_Thuoc = @ID";
            string tenDVT = null;
            string MoTa = null;
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
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(querryAddThuoc, conn);
                    cmd.Parameters.AddWithValue("@ID", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        tenDVT = reader["TenDVT"].ToString();
                        MoTa = reader["MoTaCachDung"].ToString();
                    }
                    reader.Close();

                }

                danhSachThuoc.Add(new ThuocDaChon
                {
                    ID_Thuoc = id,
                    TenThuoc = ten,
                    DonViTinh = tenDVT,
                    MoTa = MoTa,
                    DonGiaBan = donGiaBan,
                    SoLuong = soLuong
                });
            }

            dgThuocDaChon.Items.Refresh();
            txtSoLuong.Text = "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCaKham.Text) || string.IsNullOrWhiteSpace(txtTrieuChung.Text) || cboLoaiBenh.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            if (idPhieuKham == null)
            {
                // ======= CHẾ ĐỘ THÊM MỚI =======
                string query = @"INSERT INTO PHIEUKHAM (ID_TiepNhan, CaKham, TrieuChung, ID_LoaiBenh) 
                         VALUES (@ID_TiepNhan, @CaKham, @TrieuChung, @ID_LoaiBenh); 
                         SELECT SCOPE_IDENTITY();";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ID_TiepNhan", idTiepNhan);
                    cmd.Parameters.AddWithValue("@CaKham", txtCaKham.Text.Trim());
                    cmd.Parameters.AddWithValue("@TrieuChung", txtTrieuChung.Text.Trim());
                    cmd.Parameters.AddWithValue("@ID_LoaiBenh", cboLoaiBenh.SelectedValue);

                    object result = cmd.ExecuteScalar();
                    idPhieuKham = Convert.ToInt32(result);
                }
            }
            else
            {
                // ======= CHẾ ĐỘ CHỈNH SỬA =======
                string updateQuery = @"UPDATE PHIEUKHAM 
                               SET CaKham = @CaKham, TrieuChung = @TrieuChung, ID_LoaiBenh = @ID_LoaiBenh 
                               WHERE ID_PhieuKham = @ID_PhieuKham";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(updateQuery, con);
                    cmd.Parameters.AddWithValue("@CaKham", txtCaKham.Text.Trim());
                    cmd.Parameters.AddWithValue("@TrieuChung", txtTrieuChung.Text.Trim());
                    cmd.Parameters.AddWithValue("@ID_LoaiBenh", cboLoaiBenh.SelectedValue);
                    cmd.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);
                    cmd.ExecuteNonQuery();
                }

                // Xoá toa thuốc cũ trước khi thêm lại
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string querryDelToaThuoc = @"DELETE FROM TOATHUOC WHERE ID_PhieuKham = @ID_PhieuKham";
                    SqlCommand cmd = new SqlCommand(querryDelToaThuoc, con);
                    cmd.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);
                    cmd.ExecuteNonQuery();
                }

                // Cập nhật báo cáo sử dụng thuốc sau khi xóa
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmdBaoCao = new SqlCommand("TaoBaoCaoSuDungThuoc", con);
                    cmdBaoCao.CommandType = CommandType.StoredProcedure;

                    cmdBaoCao.Parameters.AddWithValue("@Thang", Thang);
                    cmdBaoCao.Parameters.AddWithValue("@Nam", Nam);
                    cmdBaoCao.ExecuteNonQuery();
                }
            }

            // ======= LƯU THÔNG TIN TOA THUỐC =======
            foreach (var thuoc in danhSachThuoc)
            {
                string insertThuoc = @"INSERT INTO TOATHUOC (ID_PhieuKham, ID_Thuoc, DonGiaBan_LucMua, SoLuong) 
                               VALUES (@ID_PhieuKham, @ID_Thuoc, @DonGiaBan, @SoLuong)";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(insertThuoc, con);
                    cmd.Parameters.AddWithValue("@ID_PhieuKham", idPhieuKham);
                    cmd.Parameters.AddWithValue("@ID_Thuoc", thuoc.ID_Thuoc);
                    cmd.Parameters.AddWithValue("@DonGiaBan", thuoc.DonGiaBan);
                    cmd.Parameters.AddWithValue("@SoLuong", thuoc.SoLuong);
                    cmd.ExecuteNonQuery();
                }
            }

            // Tạo hiệu ứng slide-out sang phải cho form hiện tại

            MessageBox.Show("Lưu thành công thông tin khám bệnh và toa thuốc.");

            // ======= Animation Slide Out + Load ExaminationList =======
            var currentTransform = new TranslateTransform();
            this.RenderTransform = currentTransform;

            var slideOut = new DoubleAnimation
            {
                From = 0,
                To = this.ActualWidth,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            slideOut.Completed += (s, _) =>
            {
                var parent = this.Parent as Border;
                if (parent != null)
                {
                    var list = new SidebarItems.ExaminationList();
                    list.RenderTransform = new TranslateTransform { X = -this.ActualWidth };

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

