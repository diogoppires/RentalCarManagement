using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class Booking
    {
        [Key]
        public int idBooking { get; set; }
        [Display(Name = "Initial Date Booking")]
        [DataType(DataType.Date)]
        public DateTime bookingInit { get; set; }


        [Display(Name = "Final Date Booking")]
        [DataType(DataType.Date)]
        public DateTime bookingEnd { get; set; }
        public virtual Vehicle vehicle { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}