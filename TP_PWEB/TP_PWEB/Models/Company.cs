using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class Company
    {
        [Key]
        public int IDCompany { get; set; }
        [Display(Name = "Company Name")]
        public string Name { get; set; }
    }
}