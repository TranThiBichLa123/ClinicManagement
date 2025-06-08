using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTO.PhanQuyen;

namespace BLL
{
    public class NhomNguoiDungBLL
    {
        private NhomNguoiDungDAL dal = new NhomNguoiDungDAL();

        public List<NhomNguoiDungDTO> GetAll()
        {
            return dal.GetAll();
        }
    }
}
