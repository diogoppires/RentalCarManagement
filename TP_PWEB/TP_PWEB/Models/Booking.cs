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
        public DateTime bookingInit { get; set; }
        public DateTime bookingEnd { get; set; }
        public virtual Vehicle vehicle { get; set; }
    }
}