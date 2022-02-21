using BLL.Models;
using Microsoft.AspNetCore.Http;
using PowerplantCodingChallengeAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerplantCodingChallengeAPI.Tools
{
    public static class Mappers
    {
        public static List<ProductionPlan> ToApiPplant(this List<ResponseBLL> resplist)
        {
            var prodList = new List<ProductionPlan>();

            foreach (var item in resplist)
            {
                prodList.Add(new ProductionPlan
                {
                    Name = item.Name,
                    P = item.P
                });
            }

            return prodList;
        }
    }
}
