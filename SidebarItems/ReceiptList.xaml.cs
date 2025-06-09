using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using BLL;
using DTO;
using iTextFont = iTextSharp.text.Font;
using iTextParagraph = iTextSharp.text.Paragraph;
using iTextPhrase = iTextSharp.text.Phrase;
using iTextTable = iTextSharp.text.pdf.PdfPTable;
using iTextCell = iTextSharp.text.pdf.PdfPCell;
using System.Data;


namespace ClinicManagement.SidebarItems
{
    public partial class ReceiptList : UserControl
    {
        private NhapThuocBLL _nhapThuocBLL = new NhapThuocBLL();
        private List<NhapThuoc.PhieuNhapThuocDTO> _allPhieuNhap = new List<NhapThuoc.PhieuNhapThuocDTO>();
        private readonly LoginLogBLL loginLogBLL = new LoginLogBLL();
        private readonly PhanQuyenBLL phanQuyenBLL = new PhanQuyenBLL();
        public string Account { get; private set; }
        public ReceiptList() { }
        public ReceiptList( string userEmail)
        {
            InitializeComponent();
            Account = userEmail;
            // Load quyền
            int nhomQuyen = phanQuyenBLL.LayNhomTheoEmail(Account);
            var danhSachQuyen = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(nhomQuyen);

            PhanQuyenHelper.DanhSachQuyen = danhSachQuyen;
            UserSession.Email = Account;
            UserSession.NhomQuyen = nhomQuyen;
            UserSession.DanhSachChucNang = danhSachQuyen;
            Loaded += ReceiptList_Loaded;
            
        }

        private void ReceiptList_Loaded(object sender, RoutedEventArgs e)
        {
          
            
            LoadPhieuNhap();
        }
        private bool HasPermission(int chucNangId)
        {
            return UserSession.DanhSachChucNang.Contains(chucNangId);
        }

        private bool DenyIfNoPermission(int chucNangId)
        {
            if (!HasPermission(chucNangId))
            {
                MessageBox.Show("Bạn không có quyền thực hiện chức năng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Tải toàn bộ danh sách phiếu nhập
        /// </summary>
        private void LoadPhieuNhap()
        {
            _allPhieuNhap = _nhapThuocBLL.GetDanhSachPhieuNhap();

            if (_allPhieuNhap.Count == 0)
                MessageBox.Show("Không có dữ liệu phiếu nhập", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            receiptDataGrid.ItemsSource = _allPhieuNhap;
        }

        /// <summary>
        /// Tìm kiếm theo ngày nhập được chọn từ DatePicker
        /// </summary>
        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            string input = textBoxSearch.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                receiptDataGrid.ItemsSource = _allPhieuNhap;
                return;
            }

            if (DateTime.TryParseExact(input, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime searchDate))
            {
                var filtered = _allPhieuNhap
                    .Where(p => p.NgayNhap.Date == searchDate)
                    .ToList();

                if (filtered.Count == 0)
                    MessageBox.Show("Không tìm thấy phiếu nhập nào cho ngày này.", "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);

                receiptDataGrid.ItemsSource = filtered;
            }
            else
            {
                MessageBox.Show("Định dạng ngày không hợp lệ. Vui lòng nhập theo định dạng dd/MM/yyyy.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        /// <summary>
        /// Mở form nhập thuốc mới và load lại danh sách
        /// </summary>
        private void AddClick(object sender, RoutedEventArgs e)
        {
            if (DenyIfNoPermission(14)) return;
           
            var newDrugWindow = new NewDrug();
            newDrugWindow.ShowDialog();

            LoadPhieuNhap();
        }


        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            // Làm mới danh sách từ database
            _allPhieuNhap = _nhapThuocBLL.GetDanhSachPhieuNhap();

            // Gán lại danh sách vào DataGrid
            receiptDataGrid.ItemsSource = _allPhieuNhap;
            receiptDataGrid.Items.Refresh();

            // Xóa nội dung ô tìm kiếm
            textBoxSearch.Text = "";

            // Thông báo nếu không có dữ liệu
            if (_allPhieuNhap.Count == 0)
            {
                MessageBox.Show("Hiện không có dữ liệu phiếu nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        /// <summary>
        /// Xử lý khi nhấn nút xem chi tiết trong bảng
        /// </summary>
        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is NhapThuoc.PhieuNhapThuocDTO selected)
            {
                var viewWindow = new NewDrug(selected.ID_PhieuNhapThuoc, true);
                viewWindow.ShowDialog();
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var selectedPhieu = (NhapThuoc.PhieuNhapThuocDTO)receiptDataGrid.SelectedItem;
            if (selectedPhieu == null)
            {
                MessageBox.Show("Vui lòng chọn một phiếu nhập để xuất.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<NhapThuoc.ChiTietPhieuNhapThuocDTO> dsChiTiet = _nhapThuocBLL.GetChiTietPhieuNhap(selectedPhieu.ID_PhieuNhapThuoc);


            if (dsChiTiet == null || dsChiTiet.Count == 0)
            {
                MessageBox.Show("Phiếu nhập không có dữ liệu chi tiết!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã xuất một phiếu nhập");

            ExportPhieuNhapPDF(selectedPhieu, dsChiTiet);
        }

        private void ExportPhieuNhapPDF(NhapThuoc.PhieuNhapThuocDTO phieu, List<NhapThuoc.ChiTietPhieuNhapThuocDTO> chiTietList)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"PhieuNhap_{phieu.ID_PhieuNhapThuoc}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                Document doc = new Document(PageSize.A4, 30, 30, 30, 30);
                PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                doc.Open();

                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextFont fontTitle = new iTextFont(baseFont, 16, iTextFont.BOLD);
                iTextFont font = new iTextFont(baseFont, 12, iTextFont.NORMAL);

                iTextParagraph title = new iTextParagraph($"PHIẾU NHẬP THUỐC #{phieu.ID_PhieuNhapThuoc}", fontTitle);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 10f;
                doc.Add(title);

                iTextParagraph info = new iTextParagraph($"Ngày nhập: {phieu.NgayNhap:dd/MM/yyyy}\nTổng tiền: {phieu.TongTienNhap:N0} VNĐ\n\n", font);
                doc.Add(info);

                iTextTable table = new iTextTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 3f, 2f, 1.5f, 2f, 2f });

                string[] headers = { "Tên thuốc", "Hạn sử dụng", "SL nhập", "Đơn giá", "Thành tiền" };
                foreach (var header in headers)
                {
                    iTextCell cell = new iTextCell(new iTextPhrase(header, font));
                    cell.BackgroundColor = new BaseColor(230, 230, 250);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                foreach (var ct in chiTietList)
                {
                    table.AddCell(new iTextPhrase(ct.TenThuoc, font));
                    table.AddCell(new iTextPhrase(ct.HanSuDung?.ToString("dd/MM/yyyy") ?? "", font));
                    table.AddCell(new iTextPhrase(ct.SoLuongNhap.ToString(), font));
                    table.AddCell(new iTextPhrase($"{ct.DonGiaNhap:N0} VNĐ", font));
                    table.AddCell(new iTextPhrase($"{ct.ThanhTien:N0} VNĐ", font));
                }

                doc.Add(table);
                doc.Close();

                MessageBox.Show("Xuất phiếu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }





    }
}
