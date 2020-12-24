using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_PWEB.Models
{
    public class Vehicle
    {
        [Key]
        public int IDVehicle { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Category { get; set; }
        public int NumberKm { get; set; }
        public float VehicleTank { get; set; }
        public bool Damages { get; set; }
        public double Price { get; set; }

        public virtual Company Company { get; set; }
    }
}