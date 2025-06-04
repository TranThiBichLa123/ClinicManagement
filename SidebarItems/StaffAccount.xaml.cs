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
using System.Windows.Controls.Primitives;


namespace ClinicManagement.SidebarItems
{
    public partial class StaffAccount : UserControl
    {
        private readonly StaffAccountBLL _bll = new StaffAccountBLL();
        private readonly ObservableCollection<DTO.StaffAccount> roleList = new ObservableCollection<DTO.StaffAccount>();
        private readonly ObservableCollection<AccountManager> accountList = new ObservableCollection<AccountManager>();
        private readonly ObservableCollection<Staff> staffList = new ObservableCollection<Staff>();


        public StaffAccount(Doctor mainWindow)
        {
            InitializeComponent();

            staffDataGrid.ItemsSource = staffList;
            LoadYearFilter();
            LoadData();
        }

        private void LoadData()
        {
            roleList.Clear();
            accountList.Clear();
            staffList.Clear();

            _bll.GetRoleList().ForEach(item => roleList.Add(item));
            foreach (var staff in _bll.GetStaffList())
            {
                staff.PropertyChanged += Staff_PropertyChanged;
                staffList.Add(staff);
            }

        }
        private void Staff_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Staff.IsSelected))
            {
                var headerCheckBox = FindHeaderCheckBox();
                if (headerCheckBox != null)
                {
                    // Nếu tất cả đều được chọn
                    if (staffList.All(s => s.IsSelected))
                        headerCheckBox.IsChecked = true;
                    // Nếu không chọn cái nào
                    else if (staffList.All(s => !s.IsSelected))
                        headerCheckBox.IsChecked = false;
                    // Nếu chọn một vài cái => trạng thái không xác định
                    else
                        headerCheckBox.IsChecked = null;
                }
            }
        }
        private CheckBox FindHeaderCheckBox()
        {
            var header = staffDataGrid.Columns[0].Header as CheckBox;
            if (header != null)
                return header;

            // Nếu dùng DataTemplate thì phải tìm trong Visual Tree
            var column = staffDataGrid.Columns[0];
            var headerPresenter = FindVisualChild<DataGridColumnHeadersPresenter>(staffDataGrid);
            if (headerPresenter != null)
            {
                var headerCell = headerPresenter.ItemContainerGenerator.ContainerFromIndex(0) as DataGridColumnHeader;
                return FindVisualChild<CheckBox>(headerCell);
            }

            return null;
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    return (T)child;
                else
                {
                    var result = FindVisualChild<T>(child);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }


        private void btnEditRole_Click(object sender, RoutedEventArgs e)
        {
            new EditRole().ShowDialog();
        }

        private void btnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddStaff();
            if (addWindow.ShowDialog() == true)
            {
                var newStaff = addWindow.NewStaff;

                MessageBox.Show("Thêm nhân viên mới thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }

        }

        private void btnEditStaff_Click(object sender, RoutedEventArgs e)
        {
            var selectedStaff = staffDataGrid.SelectedItem as Staff;
            if (selectedStaff == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên để chỉnh sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var staffCopy = new Staff
            {
                ID_NhanVien = selectedStaff.ID_NhanVien,
                HoTenNV = selectedStaff.HoTenNV,
                NgaySinh = selectedStaff.NgaySinh,
                GioiTinh = selectedStaff.GioiTinh,
                CCCD = selectedStaff.CCCD,
                DienThoai = selectedStaff.DienThoai,
                DiaChi = selectedStaff.DiaChi,
                Email = selectedStaff.Email,
                HinhAnh = selectedStaff.HinhAnh,
                ID_VaiTro = selectedStaff.ID_VaiTro,
                TrangThai = selectedStaff.TrangThai,
                MatKhau = selectedStaff.MatKhau
            };

            var editWindow = new EditStaff(staffCopy);
            if (editWindow.ShowDialog() == true)
            {
                var updated = editWindow.UpdatedStaff;

                try
                {
                    if (_bll.UpdateStaff(updated))
                    {
                        // Cách 1: Cập nhật phần tử trong danh sách (hiện đang dùng)
                        UpdateStaffInList(selectedStaff, updated);

                        // Cách 2: Nếu muốn reload toàn bộ danh sách thay vì cập nhật từng trường
                        // LoadData();

                        MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }


        private void btnDeleteStaff_Click(object sender, RoutedEventArgs e)
        {
            var selectedStaffList = staffList.Where(s => s.IsSelected).ToList();



            if (selectedStaffList.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 nhân viên để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {selectedStaffList.Count} nhân viên đã chọn không?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Lấy danh sách ID để xóa
                var idsToDelete = selectedStaffList.Select(s => s.ID_NhanVien).ToList();

                try
                {
                    if (_bll.DeleteMultipleStaff(idsToDelete))
                    {
                        // Xóa khỏi ObservableCollection để cập nhật UI
                        foreach (var s in selectedStaffList)
                            staffList.Remove(s);

                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa nhân viên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi trong quá trình xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void UpdateStaffInList(Staff original, Staff updated)
        {
            original.HoTenNV = updated.HoTenNV;
            original.NgaySinh = updated.NgaySinh;
            original.GioiTinh = updated.GioiTinh;
            original.CCCD = updated.CCCD;
            original.DienThoai = updated.DienThoai;
            original.DiaChi = updated.DiaChi;
            original.Email = updated.Email;
            original.HinhAnh = updated.HinhAnh;
            original.ID_VaiTro = updated.ID_VaiTro;
            original.TrangThai = updated.TrangThai;
        }

        private void HeaderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var staff in staffList)
            {
                staff.IsSelected = true;
            }
        }

        private void HeaderCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var staff in staffList)
            {
                staff.IsSelected = false;
            }
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = textBoxSearch.Text.Trim().ToLower();
            var category = (searchCategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filtered = _bll.GetStaffList().Where(staff =>
            {
                string hoTen = (staff.HoTenNV ?? "").ToLower();
                string maNV = staff.ID_NhanVien.ToString();
                string vaiTro = staff.ID_VaiTro.ToString();

                if (string.IsNullOrWhiteSpace(keyword))
                    return true;

                if (category == "Mã nhân viên")
                    return maNV.Contains(keyword);
                else if (category == "Họ tên")
                    return hoTen.Contains(keyword);
                else if (category == "Vai trò")
                    return vaiTro.Contains(keyword);
                else // Tất cả
                    return maNV.Contains(keyword) || hoTen.Contains(keyword) || vaiTro.Contains(keyword);
            }).ToList();

            staffList.Clear();
            foreach (var item in filtered)
            {
                item.PropertyChanged += Staff_PropertyChanged;
                staffList.Add(item);
            }
        }
        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset các ô tìm kiếm về mặc định
            textBoxSearch.Text = "";
            searchCategoryComboBox.SelectedIndex = 0;
            yearComboBox.SelectedIndex = 0;

            // Tải lại toàn bộ danh sách nhân viên
            LoadData();
        }

        private void LoadYearFilter()
        {
            int currentYear = DateTime.Now.Year;
            yearComboBox.Items.Add("Tất cả"); // Mặc định

            for (int year = 1950; year <= currentYear; year++)
            {
                yearComboBox.Items.Add(year.ToString());
            }

            yearComboBox.SelectedIndex = 0; // Chọn "Tất cả" lúc đầu
        }

        private void yearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedYear = yearComboBox.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedYear) || selectedYear == "Tất cả")
            {
                LoadData(); // Nếu chọn "Tất cả" thì reload danh sách
                return;
            }

            var filtered = _bll.GetStaffList().Where(staff =>
            {
                string staffYear = staff.NgaySinh?.Year.ToString();
                return staffYear == selectedYear;
            }).ToList();

            staffList.Clear();
            foreach (var item in filtered)
            {
                item.PropertyChanged += Staff_PropertyChanged;
                staffList.Add(item);
            }
        }






    }

}
