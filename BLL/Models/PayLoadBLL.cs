using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class PayLoadBLL
    {
        public double Load { get; set; }
        public FuelBLL Fuels { get; set; }
        public IEnumerable<PowerplantBLL> Powerplants { get; set; }
    }
}
