using BLL;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClinicManagement.SidebarItems
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : UserControl
    {
        public DashBoard()
        {
            InitializeComponent();
            LoadBenhNhanChart();
            LoadTopThuocChart();
            LoadGioiTinhChart();
            LoadDoTuoiChart();
            LoadTinhThanhChart();
            LoadDoanhThuChart();
            LoadThongTinBenhNhan();
            DataContext = this;
        }

        private void prf_MouseEnter(object sender, MouseEventArgs e)
        {
            popup_uc.PlacementTarget = prf;
            popup_uc.Placement = PlacementMode.Right;
            popup_uc.IsOpen = true;
        }

        private void prf_MouseLeave(object sender, MouseEventArgs e)
        {
            popup_uc.Visibility = Visibility.Collapsed;
            popup_uc.IsOpen = false;

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Window.GetWindow(this)?.DragMove();
            }
        }

        public SeriesCollection BenhNhanSeries { get; set; }
        public List<string> NgayLabels { get; set; }


        private ReportBLL reportBLL = new ReportBLL();

        private void LoadBenhNhanChart()
        {
            var data = reportBLL.GetThongKeBenhNhanTheoNgay();

            if (data == null || !data.Any())
            {
                BenhNhanSeries = new SeriesCollection();
                NgayLabels = new List<string>();
                return;
            }

            BenhNhanSeries = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Bệnh nhân",
            Values = new ChartValues<int>(data.Select(x => x.SoBenhNhan))
        }
    };

            NgayLabels = data.Select(x => x.Ngay.ToString("dd/MM")).ToList();
            DataContext = this;
        }

        public SeriesCollection ThuocSeries { get; set; }
        public List<string> ThuocLabels { get; set; }

        

        private void LoadTopThuocChart()
        {
            var data = reportBLL.GetTopThuocSuDungNhieuNhat(); // List<TopThuoc>

            ThuocSeries = new SeriesCollection
    {
        new RowSeries
        {
            Title = "Số lượng dùng",
            Values = new ChartValues<int>(data.Select(x => x.TongSoLuongDung))
        }
    };

            ThuocLabels = data.Select(x => x.TenThuoc).ToList();
            DataContext = this; // Cập nhật binding
        }
        public SeriesCollection GioiTinhSeries { get; set; }
        public SeriesCollection DoTuoiSeries { get; set; }
        public SeriesCollection TinhThanhSeries { get; set; }

        

        private void LoadGioiTinhChart()
        {
            var data = reportBLL.GetThongKeGioiTinh();
            GioiTinhSeries = new SeriesCollection();

            foreach (var item in data)
            {
                GioiTinhSeries.Add(new PieSeries
                {
                    Title = item.GioiTinh,
                    Values = new ChartValues<int> { item.SoLuong },
                    DataLabels = true
                });
            }

            DataContext = this;
        }

        private void LoadDoTuoiChart()
        {
            var data = reportBLL.GetThongKeDoTuoi();
            DoTuoiSeries = new SeriesCollection();

            foreach (var item in data)
            {
                DoTuoiSeries.Add(new PieSeries
                {
                    Title = item.NhomTuoi,
                    Values = new ChartValues<int> { item.SoLuong },
                    DataLabels = true
                });
            }

            DataContext = this;
        }

        private void LoadTinhThanhChart()
        {
            var data = reportBLL.GetThongKeTinhThanh();
            TinhThanhSeries = new SeriesCollection();

            foreach (var item in data)
            {
                TinhThanhSeries.Add(new PieSeries
                {
                    Title = item.DiaChi,
                    Values = new ChartValues<int> { item.SoLuong },
                    DataLabels = true
                });
            }

            DataContext = this;
        }

        public SeriesCollection DoanhThuSeries { get; set; }
        public List<string> ThangLabels { get; set; }

        private void LoadDoanhThuChart()
        {
            var data = reportBLL.GetDoanhThuTheoThang(); // List<DoanhThuTheoThang>

            DoanhThuSeries = new SeriesCollection
    {
        new ColumnSeries
        {
            Title = "Doanh thu",
            Values = new ChartValues<decimal>(data.Select(x => x.TongDoanhThu))
        }
    };

            ThangLabels = data.Select(x => $"{x.Thang}/{x.Nam}").ToList();

            DataContext = this;
        }
        public int SoBenhNhanHomNay { get; set; }

        private void LoadThongTinBenhNhan()
        {
            SoBenhNhanHomNay = reportBLL.GetSoBenhNhanHomNay();
            DataContext = this; // Cập nhật binding
        }

    }

}
