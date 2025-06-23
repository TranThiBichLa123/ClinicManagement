using System;
using System.IO;


namespace DTO.Utils
{
   


        public static class ImageHelper
        {
            
            public static string ConvertToRelativePath(string fullPath)
            {
                string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img");
                string normalizedFullPath = Path.GetFullPath(fullPath);

                if (normalizedFullPath.StartsWith(baseDir, System.StringComparison.OrdinalIgnoreCase))
                {
                    return "img" + normalizedFullPath.Substring(baseDir.Length).Replace('\\', '/');
                }

                // Nếu là đường dẫn tuyệt đối kiểu D:\THUOC\xyz.jpg thì lấy tên file và gán vào img/Thuoc
                string fileName = Path.GetFileName(fullPath);
                return "img/Thuoc/" + fileName;
            }
        }
    

}
