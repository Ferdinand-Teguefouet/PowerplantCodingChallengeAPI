using BLL.Interfaces;
using BLL.Models;
using BLL.Tools;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ResponseBllService : IResponseBll
    {
        private readonly IProductionPlanRepository _pplrepo;
        public ResponseBllService(IProductionPlanRepository pplrepository)
        {
            _pplrepo = pplrepository;
        }
        public List<ResponseBLL> GetDataFromBLL()
        {
            return _pplrepo.GetDataFromDAL().ToBllPplant();
        }
    }
}
