using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class BookingState
    {
        public Booking Booking { get; set; }
        public bool approved { get; set; }
    }
}