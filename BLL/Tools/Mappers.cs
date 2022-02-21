using BLL.Models;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Tools
{
    public static class Mappers
    {
        public static List<ResponseBLL> ToBllPplant(this List<ProductionPlan> pplist)
        {
            var respBllList = new List<ResponseBLL>();

            foreach (var item in pplist)
            {
                respBllList.Add(new ResponseBLL
                {
                    Name = item.Name,
                    P = item.P
                });
            }

            return respBllList;
        }

        public static List<ProductionPlan> ToDALpplanList(this List<ResponseBLL> list)
        {
            var ppList = new List<ProductionPlan>();

            foreach (var item in list)
            {
                ppList.Add(new ProductionPlan {
                    Name = item.Name,
                    P = item.P
                });
            }

            return ppList;

        }
    }
}
