using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class Employee
    {
        [Key]
        public int idEmployee { get; set; }

        public virtual Company idCompany{ get; set; }
        public virtual ApplicationUser idUser { get; set; }

    }
}