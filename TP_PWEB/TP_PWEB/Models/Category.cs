using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class Category
    {
        [Key]
        public int idCategory { get; set; }
        public string Name { get; set; }
        public bool isVisible { get; set; }
    }
}