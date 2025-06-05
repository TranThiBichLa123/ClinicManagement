using System.Collections.Generic;
using DAL;
using DTO;

namespace BLL
{
    public class CachDungBLL
    {
        private readonly CachDungAccess dal = new CachDungAccess();

        public List<CachDung> GetAll()
        {
            return dal.GetAll();
        }

        public bool Add(CachDung cd)
        {
            return dal.Insert(cd);
        }

        public bool Update(CachDung cd)
        {
            return dal.Update(cd);
        }

        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        public bool IsExists(string moTa)
        {
            return dal.IsExists(moTa);
        }

    }
}
