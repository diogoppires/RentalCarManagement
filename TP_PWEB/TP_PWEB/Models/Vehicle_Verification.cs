using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class Vehicle_Verification
    {
        [Key]
        public int IDVehicle_Verification { get; set; }
        public int IDVehicle { get; set; }
        public int IDVerification { get; set; }

        public virtual Vehicle Vehicle { get; set; }
        public virtual Verification Verification { get; set; }
    }
}