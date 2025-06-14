using BLL;
using DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System.IO;

using iTextFont = iTextSharp.text.Font;
using iTextParagraph = iTextSharp.text.Paragraph;
using iTextPhrase = iTextSharp.text.Phrase;
using iTextTable = iTextSharp.text.pdf.PdfPTable;
using iTextCell = iTextSharp.text.pdf.PdfPCell;

using System.Windows.Controls;

namespace ClinicManagement.SidebarItems
{
    public partial class InvoiceList : UserControl
    {
        private BillService service = new BillService();
        private readonly PhanQuyenBLL phanQuyenBLL = new PhanQuyenBLL();
        private readonly LoginLogBLL loginLogBLL = new LoginLogBLL();

        private List<HoaDon> originalList = new List<HoaDon>();
        public string Account { get; private set; }
        private Doctor _mainWindow;
        public InvoiceList() { }
        public InvoiceList(string userEmail, Doctor mainWindow)
        {
            InitializeComponent();

            Account = userEmail;
            _mainWindow = mainWindow;

            // Load quyền
            int nhomQuyen = phanQuyenBLL.LayNhomTheoEmail(Account);
            var danhSachQuyen = phanQuyenBLL.LayDanhSachIdChucNangTheoNhom(nhomQuyen);

            PhanQuyenHelper.DanhSachQuyen = danhSachQuyen;
            UserSession.Email = Account;
            UserSession.NhomQuyen = nhomQuyen;
            UserSession.DanhSachChucNang = danhSachQuyen;

            // Gắn sự kiện Loaded
            Loaded += InvoiceList_Loaded;
        }

        private void InvoiceList_Loaded(object sender, RoutedEventArgs e)
        {
            int idNhanVien = new PhanQuyenBLL().LayIDNhanVienTheoEmail(UserSession.Email);
            bool coQuyen24 = UserSession.DanhSachChucNang.Contains(28);

            

            if (coQuyen24)
            {
                // Lấy toàn bộ danh sách hóa đơn
                originalList = service.GetDanhSachHoaDon();
            }
            else
            {
                // Lấy danh sách hóa đơn do chính nhân viên đang đăng nhập tạo
                originalList = service.GetDanhSachHoaDonTheoNhanVien(idNhanVien);
            }

            billDataGrid.ItemsSource = originalList;
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

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = textBoxSearch.Text.Trim().ToLower();
            string selectedCategory = (searchCategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Tất cả";

            DateTime? selectedDate = datePickerSearch.SelectedDate;

            var filtered = originalList.Where(hd =>
            {
                bool matchCategory;

                if (selectedCategory == "Mã hóa đơn")
                    matchCategory = hd.MaHoaDon.ToString().Contains(searchText);
                else if (selectedCategory == "Mã phiếu khám")
                    matchCategory = hd.MaPhieuKham.ToString().Contains(searchText);
                else if (selectedCategory == "Mã nhân viên")
                    matchCategory = hd.MaNhanVien.ToString().Contains(searchText);
                else // "Tất cả"
                {
                    matchCategory = hd.MaHoaDon.ToString().Contains(searchText)
                                 || hd.MaPhieuKham.ToString().Contains(searchText)
                                 || hd.MaNhanVien.ToString().Contains(searchText);
                }

                bool matchDate = selectedDate == null || hd.NgayLap.Date == selectedDate.Value.Date;

                return matchCategory && matchDate;
            }).ToList();

            billDataGrid.ItemsSource = filtered;
        }

        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            textBoxSearch_TextChanged(null, null);
        }

        private void DeleteBill_Click(object sender, RoutedEventArgs e)
        {
            if (DenyIfNoPermission(27)) return;

            var bill = (sender as FrameworkElement)?.DataContext as HoaDon;
            if (bill == null) return;

            if (MessageBox.Show("Bạn có chắc muốn xóa hóa đơn này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                bool success = service.XoaHoaDon(bill.MaHoaDon);
                if (success)
                {
                    loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã xóa một hóa đơn");
                    MessageBox.Show("Đã xóa hóa đơn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    originalList.Remove(bill);
                    billDataGrid.ItemsSource = null;
                    billDataGrid.ItemsSource = originalList;
                }
                else
                {
                    MessageBox.Show("Không thể xóa hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (DenyIfNoPermission(22)) return;

            if (billDataGrid.SelectedItem is HoaDon selected)
            {
                _mainWindow.LoadUserControl(new EditBill(selected.MaPhieuKham, _mainWindow, Account));

            }
        }
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (billDataGrid.SelectedItem is HoaDon selected)
            {
                loginLogBLL.GhiLog(UserSession.Email, "Đang làm việc", 0, "Đã xuất một hóa đơn");
                var chiTiet = service.GetChiTietHoaDon(selected.MaHoaDon); // gọi từ BLL
                ExportHoaDonToPDF(selected, chiTiet);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hóa đơn để xuất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExportHoaDonToPDF(HoaDon hoaDon, List<ChiTietHoaDon> chiTietList)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"HoaDon_{hoaDon.MaHoaDon}.pdf"
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
                iTextFont boldFont = new iTextFont(baseFont, 12, iTextFont.BOLD);

                // Tiêu đề + thông tin
                iTextParagraph header = new iTextParagraph("HÓA ĐƠN THANH TOÁN", fontTitle)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                };
                doc.Add(header);

                doc.Add(new iTextParagraph("Phòng khám HOPE\nĐịa chỉ: 123 Nguyễn Văn Cừ, Q.5, TP.HCM\n\n", font));

                doc.Add(new iTextParagraph($"Mã HĐ: {hoaDon.MaHoaDon}", font));
                doc.Add(new iTextParagraph($"Ngày lập: {hoaDon.NgayLap:dd/MM/yyyy}", font));
                doc.Add(new iTextParagraph($"Bệnh nhân: {hoaDon.TenBenhNhan}\n\n", font));

                // Bảng chi tiết
                iTextTable table = new iTextTable(4);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 4f, 2f, 1.5f, 2f });

                string[] headers = { "Dịch vụ", "Đơn giá", "SL", "Thành tiền" };
                foreach (var headerText in headers)
                {
                    iTextCell cell = new iTextCell(new iTextPhrase(headerText, boldFont));
                    cell.BackgroundColor = new BaseColor(230, 230, 250);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }

                foreach (var ct in chiTietList.Where(c => c.IsVisible))
                {
                    table.AddCell(new iTextPhrase(ct.MoTa, font));
                    table.AddCell(new iTextPhrase($"{ct.DonGia:N0}", font));
                    table.AddCell(new iTextPhrase(ct.SoLuong.ToString(), font));
                    table.AddCell(new iTextPhrase($"{ct.ThanhTien:N0}", font));
                }

                doc.Add(table);

                // Tổng tiền
                iTextParagraph total = new iTextParagraph($"\nTổng cộng: {hoaDon.TongTien:N0} VNĐ", boldFont);
                total.Alignment = Element.ALIGN_RIGHT;
                total.Font.Color = BaseColor.RED;
                doc.Add(total);

                doc.Close();
                MessageBox.Show("Xuất hóa đơn thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



    }
}
