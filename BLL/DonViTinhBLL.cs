using System.Collections.Generic;
using DAL;
using DTO;

namespace BLL
{
    public class DonViTinhBLL
    {
        private readonly DonViTinhAccess access = new DonViTinhAccess();

        public List<DonViTinh> GetAll()
        {
            return access.GetAll();
        }

        public bool Add(DonViTinh dvt)
        {
            return access.Insert(dvt);
        }

        public bool Update(DonViTinh dvt)
        {
            return access.Update(dvt);
        }

        public bool Delete(int id)
        {
            return access.Delete(id);
        }
        public bool IsExists(string tenDVT)
        {
            return access.IsExists(tenDVT);
        }

    }
}
