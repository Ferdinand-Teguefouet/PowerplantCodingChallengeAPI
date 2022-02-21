using BLL.Models;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerplantCodingChallengeAPI.Models
{
    public class PayLoad
    {
        public double Load { get; set; }
        public FuelBLL Fuels { get; set; }
        public IEnumerable<PowerplantBLL> Powerplants { get; set; }
    }
}
