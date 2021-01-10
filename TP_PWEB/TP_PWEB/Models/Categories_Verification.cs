using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class Categories_Verification
    {
        [Key]
        public int IDCategories_Verification { get; set; }
        public int IDCategory { get; set; }
        public int IDVerification { get; set; }

        public virtual Category Category { get; set; }
        public virtual Verification Verification { get; set; }
    }
}