using System;
using System.Collections.Generic;
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
using DTO;
using BLL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Collections.ObjectModel;
using System.Drawing;
namespace ClinicManagement.SidebarItems
{

    public partial class Setting : UserControl
    {
        private readonly QuiDinhBLL quiDinhBLL = new QuiDinhBLL();
        private ObservableCollection<QuiDinh> quiDinhList = new ObservableCollection<QuiDinh>();
        private QuiDinh selectedQD;

        private readonly DonViTinhBLL dvtBLL = new DonViTinhBLL();
        public ObservableCollection<DonViTinh> DVTList { get; set; } = new ObservableCollection<DonViTinh>();

        private readonly CachDungBLL cachDungBLL = new CachDungBLL();
        public ObservableCollection<CachDung> CachDungList { get; set; } = new ObservableCollection<CachDung>();
        private CachDung selectedCachDung;


        private readonly LoaiBenhBLL loaiBenhBLL = new LoaiBenhBLL();
        private ObservableCollection<LoaiBenh> loaiBenhList = new ObservableCollection<LoaiBenh>();
        private LoaiBenh selectedLoaiBenh = null;
        public Setting()
        {
            InitializeComponent();
            LoadQuiDinh();
            LoadCachDung();
            LoadLoaiBenh();
            LoadDVT();
        }

        private void LoadQuiDinh()
        {
            quiDinhList = new ObservableCollection<QuiDinh>(quiDinhBLL.LayTatCa());
            gridQuiDinh.ItemsSource = quiDinhList;
        }

        private void gridQuiDinh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedQD = gridQuiDinh.SelectedItem as QuiDinh;
            if (selectedQD != null)
            {
                txtTenQuiDinh.Text = selectedQD.TenQuiDinh;
                txtGiaTriQuiDinh.Text = selectedQD.GiaTri.ToString();
            }
        }

        private void btnThemQuiDinh_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenQuiDinh.Text) || string.IsNullOrWhiteSpace(txtGiaTriQuiDinh.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            if (!double.TryParse(txtGiaTriQuiDinh.Text, out double giaTri))
            {
                MessageBox.Show("Giá trị phải là số.");
                return;
            }
            if (quiDinhBLL.IsExists(txtTenQuiDinh.Text))
            {
                MessageBox.Show("Quy định đã tồn tại. Bạn có thể chỉnh sửa nếu muốn.");
                return;
            }

            var qd = new QuiDinh
            {
                TenQuiDinh = txtTenQuiDinh.Text.Trim(),
                GiaTri = giaTri
            };

            if (quiDinhBLL.Them(qd))
            {
                MessageBox.Show("Thêm thành công!");
                LoadQuiDinh();
                ClearQuiDinhForm();
            }
            else
            {
                MessageBox.Show("Thêm thất bại.");
            }
        }

        private void btnSuaQuiDinh_Click(object sender, RoutedEventArgs e)
        {
            if (selectedQD == null)
            {
                MessageBox.Show("Chọn quy định để sửa.");
                return;
            }

            if (!double.TryParse(txtGiaTriQuiDinh.Text, out double giaTri))
            {
                MessageBox.Show("Giá trị phải là số.");
                return;
            }

            selectedQD.GiaTri = giaTri;

            if (quiDinhBLL.CapNhat(selectedQD))
            {
                MessageBox.Show("Cập nhật thành công!");
                LoadQuiDinh();
                ClearQuiDinhForm();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại.");
            }
        }

        private void btnXoaQuiDinh_Click(object sender, RoutedEventArgs e)
        {
            if (selectedQD == null)
            {
                MessageBox.Show("Chọn quy định để xoá.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xoá?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (quiDinhBLL.Xoa(selectedQD.TenQuiDinh))
                {
                    MessageBox.Show("Xoá thành công!");
                    LoadQuiDinh();
                    ClearQuiDinhForm();
                }
                else
                {
                    MessageBox.Show("Xoá thất bại.");
                }
            }
        }

        private void ClearQuiDinhForm()
        {
            txtTenQuiDinh.Clear();
            txtGiaTriQuiDinh.Clear();
            selectedQD = null;
        }

        private void LoadDVT()
        {
            DVTList.Clear();
            foreach (var item in dvtBLL.GetAll())
                DVTList.Add(item);
            gridDVT.ItemsSource = DVTList;
        }

        private void BtnThemDVT_Click(object sender, RoutedEventArgs e)
        {
            var ten = txtTenDVT.Text.Trim();
            if (string.IsNullOrEmpty(ten))
            {
                MessageBox.Show("Vui lòng nhập tên đơn vị.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dvtBLL.IsExists(ten))
            {
                MessageBox.Show("Đơn vị tính đã tồn tại. Bạn có thể chỉnh sửa nếu muốn.");
                return;
            }

            var dvt = new DonViTinh { TenDVT = ten };
            if (dvtBLL.Add(dvt))
            {
                LoadDVT();
                txtTenDVT.Clear();
            }
            else
                MessageBox.Show("Thêm thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnSuaDVT_Click(object sender, RoutedEventArgs e)
        {
            var selected = gridDVT.SelectedItem as DonViTinh;
            if (selected == null) return;

            var newTen = txtTenDVT.Text.Trim();
            if (string.IsNullOrEmpty(newTen))
            {
                MessageBox.Show("Vui lòng nhập tên đơn vị mới.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selected.TenDVT = newTen;
            if (dvtBLL.Update(selected))
            {
                LoadDVT();
                txtTenDVT.Clear();
            }
            else
                MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnXoaDVT_Click(object sender, RoutedEventArgs e)
        {
            var selected = gridDVT.SelectedItem as DonViTinh;
            if (selected == null) return;


            if (MessageBox.Show("Bạn có chắc muốn xoá?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (dvtBLL.Delete(selected.ID_DVT))
                {
                    LoadDVT();
                    txtTenDVT.Clear();
                }
                else
                    MessageBox.Show("Xoá thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void gridDVT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridDVT.SelectedItem is DonViTinh selected)
                txtTenDVT.Text = selected.TenDVT;
        }

        private void LoadCachDung()
        {
            CachDungList.Clear();
            foreach (var item in cachDungBLL.GetAll())
                CachDungList.Add(item);

            gridCachDung.ItemsSource = CachDungList;
        }

        private void BtnThemCachDung_Click(object sender, RoutedEventArgs e)
        {
            string moTa = txtCachDung.Text.Trim();
            if (string.IsNullOrEmpty(moTa))
            {
                MessageBox.Show("Vui lòng nhập mô tả cách dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cachDungBLL.IsExists(moTa))
            {
                MessageBox.Show("Mô tả cách dùng đã tồn tại. Bạn có thể chỉnh sửa nếu muốn.");
                return;
            }

            var newItem = new CachDung { MoTaCachDung = moTa };
            if (cachDungBLL.Add(newItem))
            {
                LoadCachDung();
                txtCachDung.Clear();
            }
            else
                MessageBox.Show("Thêm thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnSuaCachDung_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCachDung == null)
            {
                MessageBox.Show("Vui lòng chọn dòng để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string moTa = txtCachDung.Text.Trim();
            if (string.IsNullOrEmpty(moTa))
            {
                MessageBox.Show("Vui lòng nhập mô tả mới.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedCachDung.MoTaCachDung = moTa;
            if (cachDungBLL.Update(selectedCachDung))
            {
                LoadCachDung();
                txtCachDung.Clear();
            }
            else
                MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnXoaCachDung_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCachDung == null)
            {
                MessageBox.Show("Vui lòng chọn dòng để xoá.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xoá?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (cachDungBLL.Delete(selectedCachDung.ID_CachDung))
                {
                    LoadCachDung();
                    txtCachDung.Clear();
                }
                else
                    MessageBox.Show("Xoá thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void gridCachDung_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCachDung = gridCachDung.SelectedItem as CachDung;
            if (selectedCachDung != null)
                txtCachDung.Text = selectedCachDung.MoTaCachDung;
        }

        private void LoadLoaiBenh()
        {
            loaiBenhList.Clear();
            foreach (var item in loaiBenhBLL.GetAll())
                loaiBenhList.Add(item);

            gridLoaiBenh.ItemsSource = loaiBenhList;
        }

        private void gridLoaiBenh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedLoaiBenh = gridLoaiBenh.SelectedItem as LoaiBenh;
            if (selectedLoaiBenh != null)
            {
                txtTenBenh.Text = selectedLoaiBenh.TenLoaiBenh;
                txtTrieuChung.Text = selectedLoaiBenh.TrieuChung;
                txtHuongDieuTri.Text = selectedLoaiBenh.HuongDieuTri;
            }
        }

        private void BtnThemLoaiBenh_Click(object sender, RoutedEventArgs e)
        {

            string tenBenh = txtTenBenh.Text.Trim();
            string trieuChung = txtTrieuChung.Text.Trim();
            string huongDieuTri = txtHuongDieuTri.Text.Trim();

            if (string.IsNullOrWhiteSpace(tenBenh))
            {
                MessageBox.Show("Vui lòng nhập tên loại bệnh.");
                return;
            }

            if (loaiBenhBLL.IsExists(tenBenh))
            {
                MessageBox.Show("Loại bệnh này đã tồn tại. Bạn có thể sửa nếu muốn.");
                return;
            }

            var newBenh = new LoaiBenh
            {
                TenLoaiBenh = txtTenBenh.Text.Trim(),
                TrieuChung = txtTrieuChung.Text.Trim(),
                HuongDieuTri = txtHuongDieuTri.Text.Trim()
            };


            if (loaiBenhBLL.Insert(newBenh))
            {
                MessageBox.Show("Thêm thành công!");
                LoadLoaiBenh();
            }
            else
                MessageBox.Show("Thêm thất bại.");
        }

        private void BtnSuaLoaiBenh_Click(object sender, RoutedEventArgs e)
        {
            if (selectedLoaiBenh == null)
            {
                MessageBox.Show("Chọn loại bệnh cần sửa.");
                return;
            }

            selectedLoaiBenh.TenLoaiBenh = txtTenBenh.Text.Trim();
            selectedLoaiBenh.TrieuChung = txtTrieuChung.Text.Trim();
            selectedLoaiBenh.HuongDieuTri = txtHuongDieuTri.Text.Trim();

            if (loaiBenhBLL.Update(selectedLoaiBenh))
            {
                MessageBox.Show("Cập nhật thành công!");
                LoadLoaiBenh();
            }
            else
                MessageBox.Show("Cập nhật thất bại.");
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


        private void BtnXoaLoaiBenh_Click(object sender, RoutedEventArgs e)
        {
            if (selectedLoaiBenh == null)
            {
                MessageBox.Show("Chọn loại bệnh cần xoá.");
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xoá?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (loaiBenhBLL.Delete(selectedLoaiBenh.ID_LoaiBenh))
                {
                    MessageBox.Show("Xoá thành công!");
                    LoadLoaiBenh();
                }
                else
                    MessageBox.Show("Xoá thất bại.");
            }
        }
    }


}
