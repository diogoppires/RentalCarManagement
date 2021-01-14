using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class GetVerifications
    {
        public IEnumerable<Categories_Verification> catVer { get; set; }
        public IEnumerable<Vehicle_Verification> vehVer { get; set; }
        public bool[] ChoosenVerifications { get; set; }
    }
}