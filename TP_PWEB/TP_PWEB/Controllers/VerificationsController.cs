using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TP_PWEB.Models;

namespace TP_PWEB.Controllers
{
    public class VerificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Verifications
        public ActionResult Index()
        {
            var idUser = User.Identity.GetUserId();
            var user = db.AdminBusinesses.Where(admB => admB.idUser.Id == idUser).First();
            var listVerifications = db.Verifications.Where(v => v.Company.IDCompany == user.idCompany.IDCompany).ToList();
            return View(listVerifications);
        }

        // GET: Verifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Verification verification = db.Verifications.Find(id);
            if (verification == null)
            {
                return HttpNotFound();
            }
            return View(verification);
        }

        // GET: Verifications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Verifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDVerifications,VerificationName")] Verification verification)
        {
            if (ModelState.IsValid)
            {

                if (db.Verifications.Select(v => v.VerificationName == verification.VerificationName) != null)
                {
                    ModelState.AddModelError("name", "Verification '" + verification.VerificationName + "' already exists.");
                    return View(verification);
                }

                var idUser = User.Identity.GetUserId();
                var user = db.AdminBusinesses.Where(admB => admB.idUser.Id == idUser).First();
                var company = db.Companies.Find(user.idCompany.IDCompany);
                verification.Company = company;
                db.Verifications.Add(verification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(verification);
        }

        public ActionResult Create_Outside(string from,int? id)
        {
            ViewBag.returnDestiny = from;
            ViewBag.returnDestinyCategory = id;
            return View();
        }

        // POST: Verifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Outside([Bind(Include = "IDVerifications,VerificationName")] Verification verification, string from, int id)
        {
            if (ModelState.IsValid)
            {

                if (db.Verifications.Where(v => v.VerificationName == verification.VerificationName).Count() != 0)
                {
                    ModelState.AddModelError("name", "Verification '" + verification.VerificationName + "' already exists.");
                    ViewBag.returnDestiny = from;
                    ViewBag.returnDestinyCategory = id;
                    return View(verification);
                }


                var idUser = User.Identity.GetUserId();
                var user = db.AdminBusinesses.Where(admB => admB.idUser.Id == idUser).First();
                var company = db.Companies.Find(user.idCompany.IDCompany);
                verification.Company = company;
                db.Verifications.Add(verification);
                db.SaveChanges();
                if(from == "categories_verifications")
                {
                    return RedirectToAction("Edit", "Categories_Verification", new {id = id});
                }
                else if(from == "vehicle")
                {
                    return RedirectToAction("Create", "Vehicles");
                }
            }
            return View(verification);
        }

        // GET: Verifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Verification verification = db.Verifications.Find(id);
            if (verification == null)
            {
                return HttpNotFound();
            }
            return View(verification);
        }

        // POST: Verifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDVerifications,VerificationName")] Verification verification)
        {
            if (ModelState.IsValid)
            {
                if (db.Verifications.Select(v => v.VerificationName == verification.VerificationName) != null)
                {
                    ModelState.AddModelError("name", "Verification '" + verification.VerificationName + "' already exists.");
                    return View(verification);
                }

                db.Entry(verification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(verification);
        }

        // GET: Verifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Verification verification = db.Verifications.Find(id);
            if (verification == null)
            {
                return HttpNotFound();
            }
            return View(verification);
        }

        // POST: Verifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Verification verification = db.Verifications.Find(id);
            db.Verifications.Remove(verification);
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
