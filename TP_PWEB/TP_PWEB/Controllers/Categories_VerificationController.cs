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

namespace TP_PWEB.Controllers
{
    public class Categories_VerificationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private Company getThisUserCompany()
        {
            var idUser = User.Identity.GetUserId();
            var user = db.AdminBusinesses.Where(admB => admB.idUser.Id == idUser).First();
            var company = db.Companies.Find(user.idCompany.IDCompany);
            return company;
        }

        // GET: Categories_Verification
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Categories_Verification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ThisCategoryName = db.Categories.Find(id).Name;
            ViewBag.ThisCategoryId = db.Categories.Find(id).idCategory;

            var idUser = User.Identity.GetUserId();
            var user = db.AdminBusinesses.Where(admB => admB.idUser.Id == idUser).First();
            var listVerifications = db.Categories_Verification.Where(v => v.Company.IDCompany == user.idCompany.IDCompany);
            var filteredListVerifications = listVerifications.Where(v => v.Category.idCategory == id).ToList();

            if (listVerifications == null)
            {
                return HttpNotFound();
            }
            return View(filteredListVerifications);
        }

        private bool[] getListVerifications(Category id)
        {
            Company c = getThisUserCompany();
            var allCatVerifications = db.Categories_Verification.Where(s=> s.IDCategory == id.idCategory && s.Company.IDCompany == c.IDCompany);
            var allVerifications = db.Verifications.Where(v => v.Company.IDCompany == c.IDCompany).ToList();
            bool[] checkedCatVer = new bool[allVerifications.Count()];

            foreach(var item in allCatVerifications)
            {
                var val = allVerifications.Select((s, i) => new { i, s })
                    .Where(t => t.s.IDVerifications == item.IDVerification)
                    .Select(t => t.i)
                    .First();
                checkedCatVer[val] = true;
            }
            return checkedCatVer;
        }

        // GET: Categories_Verification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category thisCat = db.Categories.Find(id);
            ViewBag.ThisCategory = thisCat.Name;

            ModelWithAll all = new ModelWithAll();
            var company = getThisUserCompany();

            all.ListOfVerifications = db.Verifications.Where(v => v.Company.IDCompany == company.IDCompany);
            all.ChoosenVerifications = getListVerifications(thisCat);


            return View(all);
        }

        // POST: Categories_Verification/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModelWithAll modelAll, int? id)
        {
            if (ModelState.IsValid)
            {
                var company = getThisUserCompany();

                //Get the actual category being edited
                Category thisCat = db.Categories.Find(id);

                //Obtain all the Categories_Verification with this categories and delete them all.
                var allCat = db.Categories_Verification.Where(s => s.IDCategory == id && s.Company.IDCompany == company.IDCompany);
                foreach(var item in allCat)
                {
                    db.Categories_Verification.Remove(item);
                }

                var allVerifications = db.Verifications.Where(v => v.Company.IDCompany == company.IDCompany).ToList();
                //Verify the choosen verifications by the user and add them to the db.
                for (int i = 0; i < allVerifications.Count(); i++)
                {
                    //Add Verifications
                    if (modelAll.ChoosenVerifications[i] == true)
                    {
                        Categories_Verification cv = new Categories_Verification();
                        cv.Category = thisCat;
                        cv.IDCategory = thisCat.idCategory;
                        cv.IDVerification = allVerifications.ElementAt(i).IDVerifications;
                        cv.Verification = allVerifications.ElementAt(i);
                        cv.Company = company;
                        db.Categories_Verification.Add(cv);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(modelAll);
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
