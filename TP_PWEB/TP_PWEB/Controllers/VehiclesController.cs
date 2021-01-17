using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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

        //This method will get 
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
            var user = db.Users.Find(User.Identity.GetUserId());
            if(user == null)
            {
                //It is created a user in order to avoid an exception during the verification in the next statement
                user = new ApplicationUser();
            }

            //If the logged user is a business admin, its going to be shown all the vehicles associated to his company
            if(db.AdminBusinesses.Any(admB => admB.idUser.Id == user.Id))
            {
                var admBusiness = db.AdminBusinesses.Where(admB => admB.idUser.Id == user.Id).First();
                var companyVehicles = db.Vehicles.Include(v => v.Category)
                                                 .Where(v => v.Company.IDCompany == admBusiness.idCompany.IDCompany);
                return View(companyVehicles.ToList());
            }
            //If not, it is because he is a client or something else, and all the vehicles can be shown.
            else
            {
                //Get all vehicles 
                var vehicles = db.Vehicles.Include(v => v.Category);
                //Verify if dates are valid in order to avoid errors and false bookings
                if (initDate != null && endDate != null)
                {
                    ViewBag.InvalidDates = false;
                    if (DateTime.Compare(initDate.Value, endDate.Value) > 0)
                    {
                        ViewBag.InvalidDates = true;
                        ModelState.Clear();
                        return View(vehicles.ToList());
                    }

                    ViewBag.InitDateSaved = initDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.EndDateSaved = endDate.Value.ToString("dd/MM/yyyy");

                    //Get all the bookings that were selected between the given dates by the user.
                    var listOfBookings = getFilteredBookings(initDate.Value, endDate.Value);
                    IEnumerable<Vehicle> availableVehicles = db.Vehicles.ToList();

                    //If there is any booking in between the given dates, the cars should disappear temporaly from the list of available vehicles
                    if (listOfBookings.Count() != 0)
                    {
                        var bookedCars = listOfBookings.Select(s => s.vehicle);
                        availableVehicles = availableVehicles.Except(bookedCars);
                    }
                    else
                    {
                        availableVehicles = db.Vehicles;
                    }
                    ModelState.Clear();
                    return View(availableVehicles.ToList());
                }
            }
            return View(db.Vehicles.ToList());
        }

        public ActionResult IndexAdmin()
        {
            var vehicles = db.Vehicles.Include(v => v.Category);
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

        [Authorize(Roles = "Business")]
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
        public ActionResult Create([Bind(Include = "IDVehicle,Brand,Model,NumberKm,VehicleTank,Damages,Price")] Vehicle vehicle, 
            int idCategory, 
            VehicleAndVerifications tempModel,
            HttpPostedFileBase singleFile)
        {
            if (ModelState.IsValid)
            {
                var company = getThisUserCompany();
                if (!checkLicensePlateFormat(vehicle.licensePlate))
                {
                    ModelState.AddModelError("licensePlate", "The licensePlate '" + vehicle.licensePlate + "' is invalid (use one of this format 'AA-00-AA','AA-AA-00','00-AA-AA')");
                    tempModel.ListOfVerifications = db.Verifications.Where(v => v.Company.IDCompany == company.IDCompany);
                    tempModel.ChoosenVerifications = new bool[tempModel.ListOfVerifications.Count()];
                    tempModel.vehicle = vehicle;
                    ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name");
                    return View(tempModel);
                }
                if (db.Vehicles.Select(v => v.licensePlate == vehicle.licensePlate).First())
                {
                    ModelState.AddModelError("licensePlate", "The licensePlate '" + vehicle.licensePlate + "' already exists.");
                    tempModel.ListOfVerifications = db.Verifications.Where(v => v.Company.IDCompany == company.IDCompany);
                    tempModel.ChoosenVerifications = new bool[tempModel.ListOfVerifications.Count()];
                    tempModel.vehicle = vehicle;
                    ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name");
                    return View(tempModel);
                }
               

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
                string fileName = "Catalog_IMG_Vehicle_ID_" + vehicle.IDVehicle;
                if (singleFile != null && singleFile.ContentLength > 0)
                {
                    fileName += Path.GetExtension(singleFile.FileName);
                    singleFile.SaveAs(Path.Combine(Server.MapPath("~/images"), fileName));
                    vehicle.Image = fileName;
                }
                db.Entry(vehicle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name", vehicle.idCategory);
            return View(vehicle);
        }

        private bool checkLicensePlateFormat(string licensePlate)
        {
            Regex r1 = new Regex("^[A-Z]{2}-[0-9]{2}-[A-Z]{2}$");
            Regex r2 = new Regex("^[0-9]{2}-[A-Z]{2}-[A-Z]{2}$");
            Regex r3 = new Regex("^[A-Z]{2}-[A-Z]{2}-[0-9]{2}$");

            if (r1.IsMatch(licensePlate) || r2.IsMatch(licensePlate) || r3.IsMatch(licensePlate))
            {
                return true;
            }
            return false;
        }

        private bool[] getListVerifications(int? id)
        {
            Company c = getThisUserCompany();
            IQueryable<Vehicle_Verification> allVehicleVerifications = db.Vehicles_Verifications.Where(s => s.IDVehicle == id);
            var allVerifications = db.Verifications.Where(v => v.Company.IDCompany == c.IDCompany).ToList();
            bool[] checkedVehVer = new bool[allVerifications.Count()];

            foreach (var item in allVehicleVerifications)
            {
                var val = allVerifications.Select((s, i) => new { i, s })
                    .Where(t => t.s.IDVerifications == item.IDVerification)
                    .Select(t => t.i)
                    .First();
                checkedVehVer[val] = true;
            }
            return checkedVehVer;
        }

        // GET: Vehicles/Edit/5
        [Authorize(Roles = "Business")]
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

            VehicleAndVerifications modelVV = new VehicleAndVerifications();
            Company c = getThisUserCompany();
            modelVV.ListOfVerifications = db.Verifications.Where(v => v.Company.IDCompany == c.IDCompany);
            modelVV.ChoosenVerifications = getListVerifications(id);
            modelVV.idCategory = vehicle.idCategory;
            modelVV.vehicle = vehicle;
            ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name", vehicle.idCategory);
            return View(modelVV);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDVehicle,Brand,Model,NumberKm,VehicleTank,Damages,Price")] Vehicle vehicle, 
            VehicleAndVerifications modelVV, 
            int idCategory,
            HttpPostedFileBase singleFile)
        {
            if (ModelState.IsValid)
            {
                Company company = getThisUserCompany();

                if (!checkLicensePlateFormat(vehicle.licensePlate))
                {
                    ModelState.AddModelError("licensePlate", "This licensePlate '" + vehicle.licensePlate + "' is invalid (use one of this format 'AA-00-AA','AA-AA-00','00-AA-AA')");
                    modelVV.ListOfVerifications = db.Verifications.Where(v => v.Company.IDCompany == company.IDCompany);
                    modelVV.ChoosenVerifications = new bool[modelVV.ListOfVerifications.Count()];
                    modelVV.vehicle = vehicle;
                    ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name");
                    return View(modelVV);
                }
                if (db.Vehicles.Where(v => v.IDVehicle != vehicle.IDVehicle).Select(v => v.licensePlate == vehicle.licensePlate).First())
                {
                    ModelState.AddModelError("licensePlate", "This licensePlate '" + vehicle.licensePlate + "' already exists.");
                    modelVV.ListOfVerifications = db.Verifications.Where(v => v.Company.IDCompany == company.IDCompany);
                    modelVV.ChoosenVerifications = new bool[modelVV.ListOfVerifications.Count()];
                    modelVV.vehicle = vehicle;
                    ViewBag.idCategory = new SelectList(db.Categories, "idCategory", "Name");
                    return View(modelVV);
                }
                var allVeh = db.Vehicles_Verifications.Where(s => s.IDVehicle == vehicle.IDVehicle && s.Company.IDCompany == company.IDCompany);
                foreach (var item in allVeh)
                {
                    db.Vehicles_Verifications.Remove(item);
                }

                var allVerifications = db.Verifications.Where(v => v.Company.IDCompany == company.IDCompany).ToList();
                //Verify the choosen verifications by the user and add them to the db.
                for (int i = 0; i < allVerifications.Count(); i++)
                {
                    //Add Verifications
                    if (modelVV.ChoosenVerifications[i] == true)
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
                vehicle.idCategory = idCategory;

                string fileName = "Catalog_IMG_Vehicle_ID_" + vehicle.IDVehicle.ToString();
                if (singleFile != null && singleFile.ContentLength > 0)
                {
                    fileName += Path.GetExtension(singleFile.FileName);
                    singleFile.SaveAs(Path.Combine(Server.MapPath("~/images"), fileName));
                    vehicle.Image = fileName;
                }

                db.Entry(vehicle).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Vehicles/Delete/5
        [Authorize(Roles = "Business")]
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
            Booking booking = db.Bookings.Where(b => b.vehicle.IDVehicle == vehicle.IDVehicle).FirstOrDefault();
            ViewBag.booked = false;
            if (booking == null)
            {
                return View(vehicle);
            }
            if(booking.state > 0)
            {
                ViewBag.booked = true;
                return View(vehicle);
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

        public ActionResult VerificationList(int? id)
        {
            AllVerifications vL = new AllVerifications();
            IEnumerable<Vehicle_Verification> vehicles_Verification;
            if (id == null)
            {
                vehicles_Verification = db.Vehicles_Verifications.Include(v => v.Vehicle);
                return View(vL);
            }
            vehicles_Verification = db.Vehicles_Verifications.Include(v => v.Vehicle).Where(v => v.IDVehicle == id);
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicles_Verification.Count() == 0)
            {
                vL.catVer = db.Categories_Verification.Include(cv => cv.Verification).Where(v => v.IDCategory == vehicle.idCategory && v.Company.IDCompany == vehicle.Company.IDCompany).ToList();
            }
            else
            {
                vL.vehVer = vehicles_Verification.ToList();
            }
            ViewBag.CarNameAndID = string.Format("{0} {1} | {2} (ID: {3})", vehicle.Brand, vehicle.Model, vehicle.licensePlate, vehicle.IDVehicle);
            return View(vL);
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
