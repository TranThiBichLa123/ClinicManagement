using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL;
using DTO;
using LiveCharts.Wpf;
using LiveCharts;
using System.ComponentModel;
using System.Windows.Media;

namespace ClinicManagement.SidebarItems
{
    public partial class DrugView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private ObservableCollection<DTO.Drug> drugList = new ObservableCollection<DTO.Drug>();
        private DrugBLL drugBLL = new DrugBLL();

        private SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                OnPropertyChanged(nameof(SeriesCollection));
            }
        }

        private string[] _labels;
        public string[] Labels
        {
            get => _labels;
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }

        public Func<double, string> Formatter { get; set; }

        public DrugView()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection();  // ✨ important
            Labels = new string[] { };
            Formatter = value => value.ToString("N");

            drugDataGrid.ItemsSource = null;
            drugDataGrid.ItemsSource = drugList;

          /*  PopulateMonthYearComboBox();*/
            /*UpdateStatistics(); // default call*/
            LoadDrugInfo();

            DataContext = this;

        }
        private void UpdateChart(List<ThongKeTuanDTO> chartData)
        {
            if (chartData == null || !chartData.Any())
            {
                return;
            }

            // ✅ Always clear the old chart before drawing new one
            SeriesCollection.Clear();

            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Đã bán",
                Values = new ChartValues<int>(chartData.Select(x => x.DaBan))
            });

            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Tồn kho",
                Values = new ChartValues<int>(chartData.Select(x => x.TonKho))
            });

            Labels = chartData.Select(x => $"Tuần {x.Tuan}").ToArray();
        }
        /*
        private void PopulateMonthYearComboBox()
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            // Tháng 1 → 12
            for (int month = 1; month <= 12; month++)
            {
                monthComboBox.Items.Add($"Tháng {month}");
            }
            monthComboBox.SelectedIndex = currentMonth - 1;

            // Năm hiện tại đổ ngược về quá khứ (VD: 2025 → 1900)
            for (int year = currentYear; year >= 1900; year--)
            {
                yearComboBox.Items.Add(year.ToString());
            }
            yearComboBox.SelectedItem = currentYear.ToString();
        }
       */

        private void LoadDrugInfo()
        {
            var drugs = drugBLL.GetDrugList();

            drugList.Clear();
            if (drugs != null && drugs.Count > 0)
            {
                foreach (var drug in drugs)
                {
                    drugList.Add(drug);
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin thuốc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void SearchDrugButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string keyword = textBoxSearch.Text.Trim();
                string selectedFilter = (searchComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                
                if (string.IsNullOrEmpty(keyword))
                {
                    MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                List<Drug> results = new List<Drug>();

                switch (selectedFilter)
                {
                    case "Tên thuốc":
                        results = await drugBLL.SearchDrug1(keyword);

                        break;
                    case "Thành phần":
                        results = await drugBLL.SearchDrug2(keyword);
                        break;
                    case "Hạn sử dụng":
                        results = await drugBLL.SearchDrug3(keyword);
                        break;
                }

                drugList.Clear();
                if (results.Any())
                {
                    foreach (var item in results)
                    {
                        drugList.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy kết quả phù hợp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

     
        private void ExportButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadDrugInfo();


        }

        private void AddDrugButton_Click(object sender, RoutedEventArgs e)
        {
            NewDrug newDrug = new NewDrug();
            newDrug.ShowDialog();
        }


        private void DeleteDrugButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDrug = drugDataGrid.SelectedItem as DTO.Drug;

            if (selectedDrug == null)
            {
                MessageBox.Show("Vui lòng chọn một thuốc để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa thuốc \"{selectedDrug.TenThuoc}\" khỏi danh sách hiển thị?",
                                          "Xác nhận xóa thuốc",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                bool isHidden = drugBLL.DeleteDrug(selectedDrug.ID_Thuoc);

                if (isHidden)
                {
                    drugList.Remove(selectedDrug); 
                    MessageBox.Show("Thuốc đã được xóa thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Xóa thuốc thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void EditDrugButton_Click(object sender, RoutedEventArgs e)
        {

            var selectedDrug = drugDataGrid.SelectedItem as DTO.Drug;

            if (selectedDrug != null)
            {
                var editWindow = new EditDrug(selectedDrug); // Show edit window with data
                editWindow.ShowDialog();

                // Reload data after editing
                drugDataGrid.ItemsSource = new DrugBLL().GetDrugList();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một thuốc để sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DetailDrugButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDrug = drugDataGrid.SelectedItem as DTO.Drug;

            if (selectedDrug != null)
            {
                var editWindow = new DrugDetail(selectedDrug); // Show edit window with data
                editWindow.ShowDialog();

                // Reload data after editing
                drugDataGrid.ItemsSource = new DrugBLL().GetDrugList();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một thuốc để xem chi tiết.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

     /*   private void MonthCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (monthComboBox.SelectedItem != null && yearComboBox.SelectedItem != null)
            {
                UpdateStatistics();
            }
        }

        private void YearCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (monthComboBox.SelectedItem != null && yearComboBox.SelectedItem != null)
            {
                UpdateStatistics();
            }
        }

        private void UpdateStatistics()
        {
            try
            {
                string selectedMonthText = monthComboBox.SelectedItem as string;
                int selectedMonth = int.Parse(selectedMonthText.Split(' ')[1]);

                string selectedYearText = yearComboBox.SelectedItem as string;
                int selectedYear = int.Parse(selectedYearText);

                int daBan = drugBLL.GetTongSoLuongDaBan(selectedMonth, selectedYear);
                daBanTextBlock.Text = daBan.ToString();

                int tonKho = drugBLL.GetTongSoLuongTonKho(selectedMonth, selectedYear);
                tonKhoTextBlock.Text = tonKho.ToString();

                var chartData = drugBLL.GetThongKeTheoTuan(selectedMonth, selectedYear);
                UpdateChart(chartData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật thống kê: " + ex.Message);
            }
        }

      */

    }
}
