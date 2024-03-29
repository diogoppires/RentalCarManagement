﻿using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TP_PWEB.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<AdminBusiness> AdminBusinesses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Verification> Verifications { get; set; }
        public DbSet<Vehicle_Verification> Vehicles_Verifications { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<CheckedIn> CheckedIns { get; set; }
        public DbSet<CheckedOut> CheckedOuts { get; set; }
        public DbSet<CheckOut_Verification> CheckOut_Verifications { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<TP_PWEB.Models.Categories_Verification> Categories_Verification { get; set; }
    }
}