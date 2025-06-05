using System.Collections.Generic;
using DTO;
using DAL;

namespace BLL
{
    public class QuiDinhBLL
    {
        private readonly QuiDinhAccess _access = new QuiDinhAccess();

        public List<QuiDinh> LayTatCa()
        {
            return _access.GetAllQuiDinh();
        }

        public bool Them(QuiDinh qd)
        {
            return _access.InsertQD(qd);
        }

        public bool CapNhat(QuiDinh qd)
        {
            return _access.UpdateQD(qd);
        }

        public bool Xoa(string tenQuiDinh)
        {
            return _access.DeleteQD(tenQuiDinh);
        }
        public bool IsExists(string tenQuiDinh)
        {
            return _access.IsExists(tenQuiDinh);
        }

    }
}
