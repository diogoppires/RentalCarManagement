using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class CheckedIn
    {
        [Key]
        public int IDCheckedIn { get; set; }
        public int InitKm { get; set; }
        public float FuelTankStatus { get; set; }
        public bool Damages { get; set; }


        public virtual Booking Booking { get; set; }
    }
}