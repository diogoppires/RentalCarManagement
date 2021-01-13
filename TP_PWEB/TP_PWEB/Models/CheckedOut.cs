using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class CheckedOut
    {
        [Key]
        public int IDCheckedIn { get; set; }
        public int FinalKm { get; set; }
        public float FinalTankStatus{ get; set; }
        public bool Damages { get; set; }

        [MaxLength(500)]
        public string Image { get; set; }


        public virtual Booking Booking { get; set; }

    }
}