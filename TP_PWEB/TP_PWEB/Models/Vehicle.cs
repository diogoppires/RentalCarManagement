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
        [Display(Name = "ID")]
        public int IDVehicle { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        [Display(Name = "License Plate")]
        public string licensePlate { get; set; }
        [Display(Name = "Km")]
        public int NumberKm { get; set; }
        [Display(Name = "Tank")]
        public float VehicleTank { get; set; }
        public bool Damages { get; set; }
        [Display(Name = "Price/Day")]
        public double Price { get; set; }
        [MaxLength(1000)]
        public string Image { get; set; }

        public virtual Company Company { get; set; }
        public virtual Category Category { get; set; }
        public virtual int idCategory { get; set; }
    }
}