using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public enum States{
        PENDING,
        APPROVED,
        CHECKED_IN,
        CHECKED_OUT,
    }

    public class Booking
    {
        [Key]
        [Display(Name = "Booking Id")]
        public int idBooking { get; set; }
        [Display(Name = "Check_In Date")]
        [DataType(DataType.Date)]
        public DateTime bookingInit { get; set; }
        [Display(Name = "State")]
        public States state { get; set; }
        [Display(Name = "Check_Out Date")]
        [DataType(DataType.Date)]
        public DateTime bookingEnd { get; set; }

        public virtual Vehicle vehicle { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}