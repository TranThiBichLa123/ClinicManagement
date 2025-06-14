using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using BLL;
using DTO;
using static DTO.PhanQuyen;


namespace ClinicManagement.SidebarItems
{
    public partial class EditRole : Window
    {
        public ObservableCollection<ChucNangPhanQuyenVM> DanhSachChucNang { get; set; }
        private readonly ChucNangBLL chucNangBLL = new ChucNangBLL();
        private readonly NhomNguoiDungDTO selectedNhom;
        private readonly PhanQuyenBLL phanQuyenBLL = new PhanQuyenBLL();

        public EditRole(NhomNguoiDungDTO nhom)
        {
            InitializeComponent();
            selectedNhom = nhom;

            // Hiển thị thông tin nhóm
            RoleCodeTextBox.Text = selectedNhom.ID_Nhom.ToString();
            RoleNameTextBox.Text = selectedNhom.TenNhom;

            // Load danh sách chức năng (SỬA LỖI TẠI ĐÂY)
            DanhSachChucNang = new ObservableCollection<ChucNangPhanQuyenVM>(
                chucNangBLL.GetAll().Select((cn, index) => new ChucNangPhanQuyenVM // Thêm index vào Select
                {
                    ID_ChucNang = cn.ID_ChucNang,
                    TenChucNang = cn.TenChucNang,
                    DuocCapQuyen = false,
                    CapDoPhanQuyen = TaoNoiDungCapDo(index, cn.TenChucNang) // Truyền index vào
                }));

            membersDataGrid.ItemsSource = DanhSachChucNang;
            LoadPhanQuyen(selectedNhom.ID_Nhom);
        }

        

        private string TaoNoiDungCapDo(int index, string tenChucNang)
        {
            switch (index + 1)
            {
                case 1: return "1.  UI-Level (Ẩn giao diện)";
                case 13: return "2.  Logic-Level (Chặn thao tác)";
                case 28: return "3. Data-Level (Giới hạn dữ liệu)";
             
                default: return $"";
            }
        }
        private void LoadPhanQuyen(int idNhom)
        {
            foreach (var item in DanhSachChucNang)
                item.DuocCapQuyen = false;

            var quyenDaCo = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(idNhom);

            foreach (var item in DanhSachChucNang)
            {
                if (quyenDaCo.Contains(item.ID_ChucNang))
                    item.DuocCapQuyen = true;
            }

            membersDataGrid.Items.Refresh();
        }

        //private void btnLuuPhanQuyen_Click(object sender, RoutedEventArgs e)
        //{


        //    LuuPhanQuyen(selectedNhom.ID_Nhom);

        //    // Giả sử đang chỉnh quyền cho chính mình:
        //    UserSession.DanhSachChucNang = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(UserSession.NhomQuyen);

        //    // Gọi trực tiếp tới cửa sổ Doctor đang mở
        //    foreach (Window win in Application.Current.Windows)
        //    {
        //        if (win is Doctor doctorWindow)
        //        {
        //            doctorWindow.RefreshPermissions();
        //            break;
        //        }
        //    }
        //}
        private void btnLuuPhanQuyen_Click(object sender, RoutedEventArgs e)
        {
            string tenNhom = RoleNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(tenNhom))
            {
                MessageBox.Show("Vui lòng nhập tên nhóm quyền!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var danhSachQuyen = DanhSachChucNang
                .Where(x => x.DuocCapQuyen == true)
                .Select(x => x.ID_ChucNang)
                .ToList();

            if (danhSachQuyen.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một quyền!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (selectedNhom.ID_Nhom == 0)
                {
                    
                    int newID = phanQuyenBLL.ThemNhomMoiVaPhanQuyen(tenNhom, danhSachQuyen);
                    if (newID > 0)
                    {
                        MessageBox.Show("Thêm nhóm quyền thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show("Không thể thêm nhóm quyền!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    LuuPhanQuyen(selectedNhom.ID_Nhom);
                    bool ok = true;
                    if (ok)
                    {
                        MessageBox.Show("Cập nhật phân quyền thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show("Không thể cập nhật!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Nếu chỉnh quyền cho chính mình → cập nhật lại
                if (UserSession.NhomQuyen == selectedNhom.ID_Nhom)
                {
                    UserSession.DanhSachChucNang = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(UserSession.NhomQuyen);
                    foreach (Window win in Application.Current.Windows)
                    {
                        if (win is Doctor doctorWindow)
                        {
                            doctorWindow.RefreshPermissions();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LuuPhanQuyen(int idNhom)
        {
            // Đảm bảo commit thay đổi
            membersDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            membersDataGrid.CommitEdit(DataGridEditingUnit.Cell, true);

            // Debug: In toàn bộ trạng thái tích chọn
            Debug.WriteLine("=== DEBUG TRẠNG THÁI PHÂN QUYỀN ===");
            foreach (var item in DanhSachChucNang)
            {
                Debug.WriteLine($"{item.ID_ChucNang} - {item.TenChucNang}: {item.DuocCapQuyen}");
            }

            var danhSachQuyen = DanhSachChucNang
                .Where(x => x.DuocCapQuyen == true)
                .Select(x => x.ID_ChucNang)
                .ToList();

            // Debug chi tiết
            Debug.WriteLine($"Đang lưu quyền cho nhóm ID: {idNhom}");
            Debug.WriteLine("Danh sách quyền đã chọn: " + string.Join(", ", danhSachQuyen));

            if (danhSachQuyen.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một quyền!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                phanQuyenBLL.LuuPhanQuyen(idNhom, danhSachQuyen);
                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu quyền: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

}
