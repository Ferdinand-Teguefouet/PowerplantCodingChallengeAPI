using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    internal class PowerplantCost
    {
        public PowerplantName Name { get; set; }
        public PowerplantType Type { get; set; }
        public double Cost { get; set; }
    }
}
