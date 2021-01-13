using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class AllVerifications
    {
        public IEnumerable<Categories_Verification> catVer { get; set; }
        public IEnumerable<Vehicle_Verification> vehVer { get; set; }
    }
}