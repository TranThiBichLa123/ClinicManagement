using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace ClinicManagement.Utils
{
    public static class ImageHelper
    {
        public static BitmapImage LoadImage(string relativePath, string fallbackPath = "img/drugDefault.jpg")
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(basePath, relativePath);

            if (File.Exists(fullPath))
                return new BitmapImage(new Uri(fullPath, UriKind.Absolute));

            return new BitmapImage(new Uri(Path.Combine(basePath, fallbackPath), UriKind.Absolute));
        }
    }
}
