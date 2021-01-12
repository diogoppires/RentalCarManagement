using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class Verification
    {
        [Key]
        public int IDVerifications { get; set; }
        public string VerificationName { get; set; }
        public virtual Company Company { get; set; }
    }
}