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
        CARDELIVERED,
        CARRECEIVED
    }

    public class Booking
    {
        [Key]
        public int idBooking { get; set; }
        [Display(Name = "Initial Date Booking")]
        [DataType(DataType.Date)]
        public DateTime bookingInit { get; set; }
        public States state { get; set; }
        [Display(Name = "Final Date Booking")]
        [DataType(DataType.Date)]
        public DateTime bookingEnd { get; set; }
        public virtual Vehicle vehicle { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}