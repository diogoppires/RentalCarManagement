using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class ModelWithAll
    {
        public IEnumerable<Verification> ListOfVerifications { get; set; }
        public bool[] ChoosenVerifications { get; set; }
        public Categories_Verification categories_Verification { get; set; }
        public int idCategory { get; set; }
    }
}