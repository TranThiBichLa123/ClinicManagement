using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTO.PhanQuyen;

namespace BLL
{
    public class ChucNangBLL
    {
        private ChucNangDAL dal = new ChucNangDAL();

        public List<ChucNangDTO> GetAll()
        {
            return dal.GetAll();
        }
    }
}
