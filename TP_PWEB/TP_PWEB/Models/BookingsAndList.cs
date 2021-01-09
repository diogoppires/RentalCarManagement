using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class BookingsAndList
    {
        public Booking Booking { get; set; }
        public List<Booking> listBookings { get; set; }
    }
}