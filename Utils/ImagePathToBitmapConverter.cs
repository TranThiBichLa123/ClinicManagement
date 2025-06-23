using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ClinicManagement.Utils
{
    public class ImagePathToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string relativePath = value as string;

                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    // Trả về fallback nếu path null
                    return new BitmapImage(new Uri("pack://application:,,,/img/Thuoc/oresol.jpg"));
                }

                // Đường dẫn tuyệt đối tới ảnh
                string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                if (File.Exists(absolutePath))
                {
                    return new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
                }
                else
                {
                    Console.WriteLine($"❌ Không tìm thấy ảnh: {absolutePath}");
                }

                // Fallback nếu không tồn tại
                return new BitmapImage(new Uri("pack://application:,,,/img/Thuoc/oresol.jpg"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi load ảnh: {ex.Message}");
                return new BitmapImage(new Uri("pack://application:,,,/img/Thuoc/oresol.jpg"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
