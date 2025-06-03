using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class ExaminationFormDTO
    {
        public int ID_PhieuKham { get; set; }
        public string HoTenBN { get; set; }
        public string TrieuChung { get; set; }
        public string TenLoaiBenh { get; set; }
        public decimal TienKham { get; set; }
        public decimal TongTienThuoc { get; set; }
        public string CaKham { get; set; }
        public DateTime NgayTiepNhan { get; set; }
    }
}
