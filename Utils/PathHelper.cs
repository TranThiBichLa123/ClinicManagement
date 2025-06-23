using System;
using System.IO;

namespace ClinicManagement.Utils
{
    public static class PathHelper
    {
        // Tạo đường dẫn tương đối (khi lưu vào DB)
        public static string GetRelativePath(string absolutePath)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            Uri baseUri = new Uri(basePath);
            Uri fullUri = new Uri(absolutePath);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        // Tạo đường dẫn tuyệt đối (khi load lên giao diện)
        public static string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }
    }
}
