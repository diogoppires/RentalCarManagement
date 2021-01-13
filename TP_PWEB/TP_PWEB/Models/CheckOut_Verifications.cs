using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class CheckOut_Verification
    {
        [Key]
        public int IDCheckOut_Verifications { get; set; }
        public Booking Booking { get; set; }
        public Verification Verification { get; set; }
        public bool isVerified { get; set; }
    }
}