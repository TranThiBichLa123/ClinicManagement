// BLL/LoaiBenhBLL.cs
using System.Collections.Generic;
using DAL;
using DTO;

namespace BLL
{
    public class LoaiBenhBLL
    {
        private readonly LoaiBenhAccess dal = new LoaiBenhAccess();

        public List<LoaiBenh> GetAll() => dal.GetAll();

        public bool Insert(LoaiBenh loaiBenh) => dal.Insert(loaiBenh);

        public bool Update(LoaiBenh loaiBenh) => dal.Update(loaiBenh);

        public bool Delete(int id) => dal.Delete(id);

        public bool IsExists(string tenLoaiBenh)
        {
            return dal.IsExists(tenLoaiBenh);
        }

    }
}
