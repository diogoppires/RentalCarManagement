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

        private HashSet<Booking> getFilteredBookings(DateTime initDate, DateTime endDate)
        {
            var filteredBookingsInit = db.Bookings.Where(b => ((DateTime.Compare(initDate, b.bookingInit) >= 0) && (DateTime.Compare(initDate, b.bookingEnd) <= 0)));
            var filteredBookingsEnd = db.Bookings.Where(b => ((DateTime.Compare(endDate, b.bookingInit) >= 0) && (DateTime.Compare(endDate, b.bookingEnd) <= 0)));
            var filteredBookings = filteredBookingsInit.Concat(filteredBookingsEnd).ToList();
            var uniqueBookings = new HashSet<Booking>(filteredBookings);
            return uniqueBookings;
        }

        private Company getThisUserCompany()
        {
            var idUser = User.Identity.GetUserId();
            var user = db.AdminBusinesses.Where(admB => admB.idUser.Id == idUser).First();
            var company = db.Companies.Find(user.idCompany.IDCompany);
            return company;
        }

        // GET: Vehicles
        public ActionResult Index(DateTime? initDate, DateTime? endDate)
        {
            ViewBag.InvalidDates = false;
            var vehicles = db.Vehicles.Include(v => v.Category);
            if(initDate != null && endDate != null)
            {
                if (DateTime.Compare(initDate.Value, endDate.Value) > 0)
                {
                    ViewBag.InvalidDates = true;
                    ModelState.Clear();
                    return View(vehicles.ToList());
                }
                
                ViewBag.InitDateSaved = initDate.Value.ToString("dd/MM/yyyy");
                ViewBag.EndDateSaved = endDate.Value.ToString("dd/MM/yyyy");

                var listOfBookings = getFilteredBookings(initDate.Value, endDate.Value); 
                IEnumerable<Vehicle> availableVehicles = Enumerable.Empty<Vehicle>();
                if(listOfBookings.Count() != 0)
                {
                    foreach (var b in listOfBookings)
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

        // GET: Vehicles/Create
        public ActionResult Create()
        {
            ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name");
            var tempModel = new VehicleAndVerifications();
            var company = getThisUserCompany();
            tempModel.ListOfVerifications = db.Verifications.Where(v => v.Company.IDCompany == company.IDCompany);
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
                var company = getThisUserCompany();

                //Add Verifications
                var allVerifications = db.Verifications.Where(v => v.Company.IDCompany == company.IDCompany).ToList();
                for(int i = 0; i < allVerifications.Count(); i++)
                {
                    if(tempModel.ChoosenVerifications[i] == true)
                    {
                        Vehicle_Verification vv = new Vehicle_Verification();
                        vv.IDVehicle = vehicle.IDVehicle;
                        vv.IDVerification = allVerifications.ElementAt(i).IDVerifications;
                        vv.Vehicle = vehicle;
                        vv.Verification = allVerifications.ElementAt(i);
                        vv.Company = company;
                        db.Vehicles_Verifications.Add(vv);
                    }
                }

                //Add Company
                vehicle.Company = company;
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
