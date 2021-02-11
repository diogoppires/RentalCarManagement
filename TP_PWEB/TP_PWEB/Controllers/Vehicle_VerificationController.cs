using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TP_PWEB.Models;

namespace TP_PWEB.Controllers
{
    [Authorize(Roles = "Business")]

    public class Vehicle_VerificationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vehicle_Verification
        public ActionResult Index(int? id)
        {
            IEnumerable<Vehicle_Verification> vehicles_Verification;
            if (id != null)
            {
                vehicles_Verification = db.Vehicles_Verifications.Include(v => v.Vehicle).Where(v => v.IDVehicle == id);
                return View(vehicles_Verification.ToList());
            }
            vehicles_Verification = db.Vehicles_Verifications.Include(v => v.Vehicle);
            return View(vehicles_Verification.ToList());
        }

        // GET: Vehicle_Verification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle_Verification vehicle_Verification = db.Vehicles_Verifications.Find(id);
            if (vehicle_Verification == null)
            {
                return HttpNotFound();
            }
            return View(vehicle_Verification);
        }

        // GET: Vehicle_Verification/Create
        public ActionResult Create()
        {
            ViewBag.IDVehicle = new SelectList(db.Vehicles, "IDVehicle", "Brand");
            return View();
        }

        // POST: Vehicle_Verification/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDVehicle_Verification,IDVehicle,IDVerification")] Vehicle_Verification vehicle_Verification)
        {
            if (ModelState.IsValid)
            {
                db.Vehicles_Verifications.Add(vehicle_Verification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDVehicle = new SelectList(db.Vehicles, "IDVehicle", "Brand", vehicle_Verification.IDVehicle);
            return View(vehicle_Verification);
        }

        // GET: Vehicle_Verification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle_Verification vehicle_Verification = db.Vehicles_Verifications.Find(id);
            if (vehicle_Verification == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDVehicle = new SelectList(db.Vehicles, "IDVehicle", "Brand", vehicle_Verification.IDVehicle);
            return View(vehicle_Verification);
        }

        // POST: Vehicle_Verification/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDVehicle_Verification,IDVehicle,IDVerification")] Vehicle_Verification vehicle_Verification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vehicle_Verification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDVehicle = new SelectList(db.Vehicles, "IDVehicle", "Brand", vehicle_Verification.IDVehicle);
            return View(vehicle_Verification);
        }

        // GET: Vehicle_Verification/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle_Verification vehicle_Verification = db.Vehicles_Verifications.Find(id);
            if (vehicle_Verification == null)
            {
                return HttpNotFound();
            }
            return View(vehicle_Verification);
        }

        // POST: Vehicle_Verification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vehicle_Verification vehicle_Verification = db.Vehicles_Verifications.Find(id);
            db.Vehicles_Verifications.Remove(vehicle_Verification);
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
