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
using BLL;
using DTO;
using System.Web.UI.HtmlControls;

namespace ClinicManagement.SidebarItems
{
    public partial class ExaminationForm : UserControl
    {
        private int idTiepNhan;
        private int idBenhNhan;
        private int? idPhieuKham = null;
        private ExaminationFormBLL bll = new ExaminationFormBLL();
        private ObservableCollection<ThuocDaChon> danhSachThuoc = new ObservableCollection<ThuocDaChon>();
        public ExaminationForm(string idBenhNhan, int idTiepNhan)
        {
            InitializeComponent();
            tblTitle.Text = "Tạo Phiếu Khám";
            this.idBenhNhan = Convert.ToInt32(idBenhNhan);
            this.idTiepNhan = idTiepNhan;
            txtMaBenhNhan.Text = idBenhNhan;

            var bn = bll.GetBenhNhanInfo(this.idBenhNhan);
            if (bn != null)
                txtTenBenhNhan.Text = bn["HoTenBN"].ToString();
            else
                MessageBox.Show("Không tìm thấy thông tin bệnh nhân.");

            cboLoaiBenh.ItemsSource = bll.GetLoaiBenh().DefaultView;
            cboLoaiBenh.DisplayMemberPath = "TenLoaiBenh";
            cboLoaiBenh.SelectedValuePath = "ID_LoaiBenh";

            cboThuoc.ItemsSource = bll.GetThuocList().DefaultView;
            cboThuoc.DisplayMemberPath = "TenThuoc";
            cboThuoc.SelectedValuePath = "ID_Thuoc";

            dgThuocDaChon.ItemsSource = danhSachThuoc;
        }

        public ExaminationForm(string idBN, int idTN, int idPK)
        {
            InitializeComponent();
            tblTitle.Text = "Sửa Phiếu Khám";
            this.idBenhNhan = Convert.ToInt32(idBN);
            this.idTiepNhan = idTN;
            this.idPhieuKham = idPK;
            txtMaBenhNhan.Text = this.idBenhNhan.ToString();

            var bn = bll.GetBenhNhanInfo(this.idBenhNhan);
            if (bn != null)
                txtTenBenhNhan.Text = bn["HoTenBN"].ToString();
            else
                MessageBox.Show("Không tìm thấy thông tin bệnh nhân.");

            cboLoaiBenh.ItemsSource = bll.GetLoaiBenh().DefaultView;
            cboLoaiBenh.DisplayMemberPath = "TenLoaiBenh";
            cboLoaiBenh.SelectedValuePath = "ID_LoaiBenh";

            cboThuoc.ItemsSource = bll.GetThuocList().DefaultView;
            cboThuoc.DisplayMemberPath = "TenThuoc";
            cboThuoc.SelectedValuePath = "ID_Thuoc";

            var pk = bll.GetPhieuKham(idPK);
            if (pk != null)
            {
                txtTrieuChung.Text = pk["TrieuChung"].ToString();
                txtCaKham.Text = pk["CaKham"].ToString();
                txtTenBenhNhan.Text = pk["HoTenBN"].ToString();
                cboLoaiBenh.SelectedValue = pk["ID_LoaiBenh"];
            }

            var toa = bll.GetToaThuoc(idPK);
            danhSachThuoc.Clear();
            foreach (DataRow row in toa.Rows)
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



        private void btnThemThuoc_Click(object sender, RoutedEventArgs e)
        {
            if (cboThuoc.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn thuốc trước khi thêm.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedThuoc = cboThuoc.SelectedItem as DataRowView;
            string id = selectedThuoc["ID_Thuoc"].ToString();
            string ten = selectedThuoc["TenThuoc"].ToString();
            decimal donGia = Convert.ToDecimal(selectedThuoc["DonGiaBan"]);
            int ton = Convert.ToInt32(selectedThuoc["SoLuongTon"]);

            if (!int.TryParse(txtSoLuong.Text.Trim(), out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng hợp lệ (> 0).", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (soLuong > ton)
            {
                MessageBox.Show($"Số lượng vượt quá tồn kho ({ton}).", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var daTonTai = danhSachThuoc.FirstOrDefault(t => t.ID_Thuoc == id);
            if (daTonTai != null)
            {
                if (daTonTai.SoLuong + soLuong > ton)
                {
                    MessageBox.Show($"Tổng số lượng vượt tồn kho ({ton}).", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                daTonTai.SoLuong += soLuong;
            }
            else
            {
                var chiTiet = bll.GetChiTietThuoc(id);
                danhSachThuoc.Add(new ThuocDaChon
                {
                    ID_Thuoc = id,
                    TenThuoc = ten,
                    DonViTinh = chiTiet["TenDVT"].ToString(),
                    MoTa = chiTiet["MoTaCachDung"].ToString(),
                    DonGiaBan = donGia,
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
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (idPhieuKham == null)
            {
                // ======= CHẾ ĐỘ THÊM MỚI =======
                idPhieuKham = bll.TaoPhieuKham(idTiepNhan, txtCaKham.Text, txtTrieuChung.Text, (int)cboLoaiBenh.SelectedValue);
            }
            else
            {
                // ======= CHẾ ĐỘ CHỈNH SỬA =======
                bll.CapNhatPhieuKham(idPhieuKham.Value, txtCaKham.Text, txtTrieuChung.Text, (int)cboLoaiBenh.SelectedValue);
                bll.XoaToaThuoc(idPhieuKham.Value);
            }

            // ======= LƯU THÔNG TIN TOA THUỐC =======
            bll.ThemToaThuoc(idPhieuKham.Value, danhSachThuoc.ToList());
            MessageBox.Show("Lưu thành công thông tin khám bệnh và toa thuốc.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            AnimateBack();
        }

        private void AnimateBack()
        {
            var slideOut = new DoubleAnimation(0, this.ActualWidth, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase()
            };
            var trans = new TranslateTransform();
            this.RenderTransform = trans;
            slideOut.Completed += (s, _) =>
            {
                if (this.Parent is Border parent)
                {
                    var list = new SidebarItems.ExaminationList();
                    list.RenderTransform = new TranslateTransform { X = -this.ActualWidth };
                    parent.Child = list;
                    var slideIn = new DoubleAnimation(-this.ActualWidth, 0, TimeSpan.FromMilliseconds(300));
                    (list.RenderTransform as TranslateTransform).BeginAnimation(TranslateTransform.XProperty, slideIn);
                }
            };
            trans.BeginAnimation(TranslateTransform.XProperty, slideOut);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e) => AnimateBack();


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
            
        }
    }
}