using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TP_PWEB.Models;

namespace TP_PWEB.Views.Vehicles
{
    public class VehiclesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vehicles
        public ActionResult Index(DateTime? bookingDate)
        {
            var vehicles = db.Vehicles.Include(v => v.Category);
            if(bookingDate != null)
            {
                ViewBag.bookingDateSaved = bookingDate.Value.ToString("dd/MM/yyyy");
                var listOfBookings = db.Bookings.Where(b => (DateTime.Compare(bookingDate.Value, b.bookingInit) >= 0) &&
                                                            (DateTime.Compare(bookingDate.Value, b.bookingEnd) <= 0));

                IEnumerable<Vehicle> availableVehicles = Enumerable.Empty<Vehicle>();
                if(listOfBookings.Count() != 0)
                {
                    foreach (Booking b in listOfBookings)
                    {
                        availableVehicles = db.Vehicles.Where(av => av.IDVehicle != b.vehicle.IDVehicle);
                    }
                }
                else
                {
                    availableVehicles = db.Vehicles;
                }
                
                ModelState.Clear();
                return View(availableVehicles.ToList());
            }
            return View(vehicles.ToList());
        }

        // GET: Vehicles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        private Company getCompany(string currentUserId)
        {
            return db.AdminBusinesses.Where(s => s.idUser.Id == currentUserId).Select(s => s.idCompany).First();
        }

        // GET: Vehicles/Create
        public ActionResult Create()
        {
            ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name");
            var tempModel = new VehicleAndVerifications();
            tempModel.ListOfVerifications = db.Verifications.ToList();
            tempModel.ChoosenVerifications = new bool[tempModel.ListOfVerifications.Count()];
            return View(tempModel);
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDVehicle,Brand,Model,NumberKm,VehicleTank,Damages,Price")] Vehicle vehicle, int idCategory, VehicleAndVerifications tempModel)
        {
            if (ModelState.IsValid)
            {
                //Add Verifications
                var allVerifications = db.Verifications.ToList();
                for(int i = 0; i < tempModel.ChoosenVerifications.Count(); i++)
                {
                    if(tempModel.ChoosenVerifications[i] == true)
                    {
                        Vehicle_Verification vv = new Vehicle_Verification();
                        vv.IDVehicle = vehicle.IDVehicle;
                        vv.IDVerification = allVerifications.ElementAt(i).IDVerifications;
                        vv.Vehicle = vehicle;
                        vv.Verification = allVerifications.ElementAt(i);
                        db.Vehicles_Verifications.Add(vv);
                    }
                }

                //Add Company
                vehicle.Company = getCompany(User.Identity.GetUserId());
                //Add Category
                vehicle.idCategory = idCategory;
                //Add Vehicle to database
                db.Vehicles.Add(vehicle);
                //Save changes
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name", vehicle.idCategory);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name", vehicle.idCategory);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDVehicle,Brand,Model,NumberKm,VehicleTank,Damages,Price,idCategory")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vehicle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name", vehicle.idCategory);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vehicle vehicle = db.Vehicles.Find(id);
            db.Vehicles.Remove(vehicle);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
