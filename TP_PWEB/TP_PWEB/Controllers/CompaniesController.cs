using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TP_PWEB.Models;

namespace TP_PWEB.Controllers
{
    public class CompaniesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Companies
        public ActionResult Index()
        {
            return View(db.Companies.ToList());
        }

        private IEnumerable<Employee> GetAllEmployees(Company company)
        {
            return db.Employees.Where(e => e.idCompany.IDCompany == company.IDCompany);
        }

        private IEnumerable<AdminBusiness> GetAllAdmBusiness(Company company)
        {
            return db.AdminBusinesses.Where(admB => admB.idCompany.IDCompany == company.IDCompany);
        }

        private int GetNumPendingBookings(Company company)
        {
            return db.Bookings.Where(b => b.state == States.PENDING && b.vehicle.Company.IDCompany == company.IDCompany).Count();
        }

        private int GetNumApprovedBookings(Company company)
        {
            return db.Bookings.Where(b => b.state == States.APPROVED && b.vehicle.Company.IDCompany == company.IDCompany).Count();
        }

        private int GetNumCheckedInBookings(Company company)
        {
            return db.Bookings.Where(b => b.state == States.CHECKED_IN && b.vehicle.Company.IDCompany == company.IDCompany).Count();
        }

        private int GetNumCheckedOutBookings(Company company)
        {
            return db.Bookings.Where(b => b.state == States.CHECKED_OUT && b.vehicle.Company.IDCompany == company.IDCompany).Count();
        }

        private int GetNumVehicles(Company company)
        {
            return db.Vehicles.Where(v => v.Company.IDCompany == company.IDCompany).Count();
        }

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }

            AllCompanyDetails allCD = new AllCompanyDetails();
            allCD.allEmployes = GetAllEmployees(company).ToList();
            allCD.allAdmBusinesses = GetAllAdmBusiness(company).ToList();
            allCD.num_pendingBookings = GetNumPendingBookings(company);
            allCD.num_approvedBookings = GetNumApprovedBookings(company);
            allCD.num_checkedInBookings = GetNumCheckedInBookings(company);
            allCD.num_checkedOutBookings = GetNumCheckedOutBookings(company);
            allCD.num_vehicles = GetNumVehicles(company);

            return View(allCD);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCompany,IDClient,Name")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Companies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCompany,IDClient,Name")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        private void deleteEmployeesAccounts(Company company)
        {
            var listEmployees = db.Employees.ToList();
            var deletedEmployes = listEmployees.Where(s => s.idCompany.IDCompany == company.IDCompany);
            foreach (var employee in deletedEmployes)
            {
                db.Users.Remove(employee.idUser);
                db.Employees.Remove(employee);
                db.SaveChanges();
            }
        }

        private void deleteVerifications(int idCompany)
        {
            var listVerifications = db.Verifications.Where(s => s.Company.IDCompany == idCompany).ToList();
            foreach (var ver in listVerifications)
            {
                db.Verifications.Remove(ver);
                db.SaveChanges();
            }
        }

        private void deleteBusinessAdms(Company company)
        {
            var listAdmBusiness = db.AdminBusinesses.Where(admB => admB.idCompany.IDCompany == company.IDCompany).ToList();
            foreach (var adminBusiness in listAdmBusiness)
            {
                db.Users.Remove(adminBusiness.idUser);
                db.AdminBusinesses.Remove(adminBusiness);
                db.SaveChanges();
            }
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            deleteBusinessAdms(company);
            deleteEmployeesAccounts(company);
            deleteVerifications(id);
            db.Companies.Remove(company);
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
