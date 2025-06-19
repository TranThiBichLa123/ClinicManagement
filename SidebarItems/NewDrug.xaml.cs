using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BLL;
using DTO;
using Microsoft.Win32;
using static DTO.NhapThuoc;

namespace ClinicManagement.SidebarItems
{
    public partial class NewDrug : Window
    {
        private readonly QuiDinhBLL quyDinhBLL = new QuiDinhBLL();

        private ObservableCollection<ChiTietPhieuNhapThuocDTO> danhSachThuocNhap = new ObservableCollection<ChiTietPhieuNhapThuocDTO>();
        private NhapThuocBLL bll = new NhapThuocBLL();

        private ThuocDTO thuocDangChon = null;
        private string selectedImagePath = "/img/drugDefault.jpg";
        private List<DonViTinhDTO> danhSachDVT;
        private List<CachDungDTO> danhSachCachDung;
        private readonly LoginLogBLL loginLogBLL = new LoginLogBLL();

        public NewDrug()
        {
            InitializeComponent();
            drugDataGrid.ItemsSource = danhSachThuocNhap;
            LoadTenThuoc();
            LoadDVT_CachDung();
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTyLeGiaBanMacDinh();
        }

        private void LoadTyLeGiaBanMacDinh()
        {
            try
            {
                var tyLe = quyDinhBLL.LayTyLeGiaBan(); // hoặc từ DAL
                textBoxTyLeGiaBan.Text = tyLe.ToString("0.##"); // định dạng số gọn
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải tỷ lệ giá bán: " + ex.Message);
            }
        }


        private void LoadDVT_CachDung()
        {
            danhSachDVT = bll.GetAllDVT();
            danhSachCachDung = bll.GetAllCachDung();
            DvtcomboBox.ItemsSource = danhSachDVT;
            DvtcomboBox.DisplayMemberPath = "TenDVT";
            DvtcomboBox.SelectedValuePath = "ID_DVT";
            CachDungcomboBox.ItemsSource = danhSachCachDung;
            CachDungcomboBox.DisplayMemberPath = "MoTaCachDung";
            CachDungcomboBox.SelectedValuePath = "ID_CachDung";
        }

        private void LoadTenThuoc()
        {
            TenThuoccomboBox.ItemsSource = bll.GetDanhSachTenThuoc();
        }

        private void TenThuoccomboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string tenThuoc = TenThuoccomboBox.Text.Trim();
            if (string.IsNullOrEmpty(tenThuoc)) return;
            var thuoc = bll.GetThuocByTen(tenThuoc);
            if (thuoc != null)
            {
                thuocDangChon = thuoc;
                DvtcomboBox.SelectedValue = thuoc.ID_DVT;
                CachDungcomboBox.SelectedValue = thuoc.ID_CachDung;
                ThanhPhancomboBox.Text = thuoc.ThanhPhan;
                XuatXucomboBox.Text = thuoc.XuatXu;
                textBoxDonGiaNhap.Text = thuoc.DonGiaNhap.ToString();
                selectedImagePath = !string.IsNullOrEmpty(thuoc.HinhAnh) ? thuoc.HinhAnh : "/img/drugDefault.jpg";
                imgThuoc.Source = new BitmapImage(new Uri(selectedImagePath, UriKind.RelativeOrAbsolute));
            }
            else thuocDangChon = null;
        }

        private void ChonThuoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tenThuoc = TenThuoccomboBox.Text.Trim();
                if (!int.TryParse(textBoxSoLuongNhap.Text, out int soLuong) || soLuong <= 0)
                {
                    MessageBox.Show("Số lượng nhập phải là số nguyên dương!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(textBoxDonGiaNhap.Text, out decimal donGia) || donGia <= 0)
                {
                    MessageBox.Show("Đơn giá nhập phải là số lớn hơn 0!", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                var ct = new ChiTietPhieuNhapThuocDTO
                {
                    TenThuoc = tenThuoc,
                    ID_Thuoc = thuocDangChon?.ID_Thuoc ?? 0,
                    SoLuongNhap = soLuong,
                    DonGiaNhap = donGia,
                    HanSuDung = datePickerHanSuDung.SelectedDate,
                    HinhAnh = selectedImagePath.Contains(":\\") ? selectedImagePath : null,
                    ID_DVT = (int)(DvtcomboBox.SelectedValue ?? thuocDangChon?.ID_DVT ?? 0),
                    ID_CachDung = (int)(CachDungcomboBox.SelectedValue ?? thuocDangChon?.ID_CachDung ?? 0),
                    ThanhPhan = ThanhPhancomboBox.Text,
                    XuatXu = XuatXucomboBox.Text,
                };

                if (ct.ID_DVT == 0 || ct.ID_CachDung == 0)
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ Đơn vị tính và Cách dùng cho thuốc mới!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                danhSachThuocNhap.Add(ct);
                drugDataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn thuốc: " + ex.Message);
            }
        }

        private void addDrug_Click(object sender, RoutedEventArgs e)
        {
            if (danhSachThuocNhap.Count == 0)
            {
                MessageBox.Show("Danh sách thuốc đang trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime ngayNhap = DateTime.Now;
            int idPhieu = bll.CreatePhieuNhapThuoc(ngayNhap);

            foreach (var ct in danhSachThuocNhap)
            {
                var thuocDB = bll.GetThuocByTen(ct.TenThuoc);

                if (thuocDB != null)
                {
                    thuocDB.DonGiaNhap = ct.DonGiaNhap;
                    thuocDB.ThanhPhan = ct.ThanhPhan;
                    thuocDB.XuatXu = ct.XuatXu;
                    thuocDB.HinhAnh = ct.HinhAnh;
                    bll.UpdateThuocAndIncreaseQuantity(thuocDB, ct.SoLuongNhap);
                    ct.ID_Thuoc = thuocDB.ID_Thuoc;
                }
                else
                {
                    var newThuoc = new ThuocDTO
                    {
                        TenThuoc = ct.TenThuoc,
                        ID_DVT = ct.ID_DVT,
                        ID_CachDung = ct.ID_CachDung,
                        ThanhPhan = ct.ThanhPhan,
                        XuatXu = ct.XuatXu,
                        DonGiaNhap = ct.DonGiaNhap,
                        HinhAnh = ct.HinhAnh
                    };
                    ct.ID_Thuoc = bll.AddNewThuoc(newThuoc);
                }

                ct.ID_PhieuNhapThuoc = idPhieu;
                bll.AddChiTietPhieuNhap(ct);
            }
            loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã thêm một phiếu nhập");
            MessageBox.Show("Đã nhập thuốc thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            danhSachThuocNhap.Clear();
            drugDataGrid.Items.Refresh();
            Reset_Click(null, null);
            LoadTenThuoc();
            LoadDVT_CachDung();
            thuocDangChon = null;
        }

        private void XoaThuoc_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is ChiTietPhieuNhapThuocDTO ct)
            {
                danhSachThuocNhap.Remove(ct);
                drugDataGrid.Items.Refresh();
            }
        }

        private void SuaThuoc_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is ChiTietPhieuNhapThuocDTO ct)
            {
                var thuocDayDu = bll.GetThuocByTen(ct.TenThuoc);

                TenThuoccomboBox.Text = ct.TenThuoc;
                DvtcomboBox.SelectedValue = thuocDayDu?.ID_DVT ?? ct.ID_DVT;
                CachDungcomboBox.SelectedValue = thuocDayDu?.ID_CachDung ?? ct.ID_CachDung;
                ThanhPhancomboBox.Text = thuocDayDu?.ThanhPhan ?? ct.ThanhPhan;
                XuatXucomboBox.Text = thuocDayDu?.XuatXu ?? ct.XuatXu;
                textBoxDonGiaNhap.Text = (thuocDayDu?.DonGiaNhap ?? ct.DonGiaNhap).ToString();
                textBoxSoLuongNhap.Text = ct.SoLuongNhap.ToString();
                datePickerHanSuDung.SelectedDate = ct.HanSuDung;

                selectedImagePath = thuocDayDu?.HinhAnh ?? ct.HinhAnh ?? "/img/drugDefault.jpg";
                imgThuoc.Source = new BitmapImage(new Uri(selectedImagePath, UriKind.RelativeOrAbsolute));

                danhSachThuocNhap.Remove(ct);
                drugDataGrid.Items.Refresh();
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            TenThuoccomboBox.Text = "";
            DvtcomboBox.SelectedIndex = -1;
            CachDungcomboBox.SelectedIndex = -1;
            ThanhPhancomboBox.Text = "";
            XuatXucomboBox.Text = "";
            textBoxSoLuongNhap.Text = "";
            textBoxDonGiaNhap.Text = "";
            textBoxTyLeGiaBan.Text = "";
            datePickerHanSuDung.SelectedDate = null;
            selectedImagePath = "/img/drugDefault.jpg";
            imgThuoc.Source = new BitmapImage(new Uri(selectedImagePath, UriKind.RelativeOrAbsolute));
            LoadTyLeGiaBanMacDinh();
        }

        private void NewDrugImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
            if (dialog.ShowDialog() == true)
            {
                selectedImagePath = dialog.FileName;
                imgThuoc.Source = new BitmapImage(new Uri(selectedImagePath, UriKind.Absolute));
            }
        }

        private readonly bool isReadOnlyMode = false;

        public NewDrug(int idPhieuNhap, bool isReadOnly = false)
        {
            InitializeComponent();
            this.isReadOnlyMode = isReadOnly;

            drugDataGrid.ItemsSource = danhSachThuocNhap;

            LoadTenThuoc();
            LoadDVT_CachDung();
            LoadPhieuNhapChiTiet(idPhieuNhap);

            if (isReadOnlyMode)
                EnableReadOnlyMode();
        }

        private void LoadPhieuNhapChiTiet(int idPhieu)
        {
            var danhSach = bll.GetChiTietPhieuNhap(idPhieu);
            danhSachThuocNhap.Clear();

            foreach (var item in danhSach)
            {
                danhSachThuocNhap.Add(item);
            }

            drugDataGrid.Items.Refresh();
        }

        private void EnableReadOnlyMode()
        {
            TenThuoccomboBox.IsEnabled = false;
            DvtcomboBox.IsEnabled = false;
            CachDungcomboBox.IsEnabled = false;
            ThanhPhancomboBox.IsEnabled = false;
            XuatXucomboBox.IsEnabled = false;
            textBoxDonGiaNhap.IsEnabled = false;
            textBoxSoLuongNhap.IsEnabled = false;
            textBoxTyLeGiaBan.IsEnabled = false;
            datePickerHanSuDung.IsEnabled = false;

            btnChonThuoc.IsEnabled = false;
            btnReset.IsEnabled = false;
            btnNhapThuoc.IsEnabled = false;
            btnChonAnhThuoc.IsEnabled = false;

            drugDataGrid.SelectionChanged += DrugDataGrid_SelectionChanged;

            foreach (var column in drugDataGrid.Columns)
            {
                if (column.Header?.ToString() == "Sửa" || column.Header?.ToString() == "Xóa")
                {
                    column.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void DrugDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (drugDataGrid.SelectedItem is ChiTietPhieuNhapThuocDTO ct)
            {
                var thuoc = bll.GetThuocByTen(ct.TenThuoc);
                if (thuoc != null)
                {
                    TenThuoccomboBox.Text = thuoc.TenThuoc;
                    DvtcomboBox.SelectedValue = thuoc.ID_DVT;
                    CachDungcomboBox.SelectedValue = thuoc.ID_CachDung;
                    ThanhPhancomboBox.Text = thuoc.ThanhPhan;
                    XuatXucomboBox.Text = thuoc.XuatXu;
                    textBoxDonGiaNhap.Text = thuoc.DonGiaNhap.ToString();
                    textBoxSoLuongNhap.Text = ct.SoLuongNhap.ToString();
                    textBoxTyLeGiaBan.Text = thuoc.TyLeGiaBan.ToString();
                    datePickerHanSuDung.SelectedDate = ct.HanSuDung;

                    if (!string.IsNullOrEmpty(thuoc.HinhAnh))
                    {
                        imgThuoc.Source = new BitmapImage(new Uri(thuoc.HinhAnh, UriKind.RelativeOrAbsolute));
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }
    }
}
