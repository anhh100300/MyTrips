using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using System.Transactions;


namespace WebApplication.Controllers
{
    [Authorize(Roles = "phongvo@2728")]
    public class SanPhamsController : Controller
    {
        private CsK24_MyTripEntities db = new CsK24_MyTripEntities();

        // GET: ves
        public ActionResult Index()
        {
            var model = db.SanPham.ToList();
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Index2()
        {
            var model = db.SanPham.ToList();
            return View(model);

        }
        [AllowAnonymous]
        public ActionResult Search(string keyword)
        {
            var model = db.SanPham.ToList();
            model = model.Where(p => p.ThongTinSanPham.ToLower().Contains(keyword.ToLower())).ToList();
            ViewBag.Keyword = keyword;
            return View("Index2", model);
        }

        [AllowAnonymous]
        // GET: ves/Details/5
        public ActionResult Details(int id)
        {
            var model = db.SanPham.Find(id);

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: SanPhams/Create
        public ActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Picture(int MaSanPham)
        {
            var path = Server.MapPath(PICTURE_PATH);
            return File(path + MaSanPham, "images");
        }

        // GET: ves/Create

        // POST: SanPhams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SanPham model, HttpPostedFileBase picture)
        {
            Validateve(model);
            if (ModelState.IsValid)
            {
                if (picture != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        db.SanPham.Add(model);
                        db.SaveChanges();

                        var path = Server.MapPath(PICTURE_PATH);
                        picture.SaveAs(path + model.MaSanPham);
                        scope.Complete();
                        return RedirectToAction("Index");
                    }

                }
                else ModelState.AddModelError("", "Picture not found!");
            }

            return View(model);
        }
        private const string PICTURE_PATH = "~/Upload/Product/";
        private void Validateve(SanPham sanPham)
        {
            if (sanPham.GiaTien < 0)
                ModelState.AddModelError("GiaTien", "Giá vé nhỏ hơn 0 ! Vui lòng nhập lại ");
        }

        // GET: SanPhams/Edit/5
        // GET: ves/Edit/5
        public ActionResult Edit(int id)
        {

            var model = db.SanPham.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        // POST: ves/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SanPham model, HttpPostedFileBase picture)
        {
            Validateve(model);
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    if (picture != null)
                    {
                        var path = Server.MapPath(PICTURE_PATH);
                        picture.SaveAs(path + model.MaSanPham);
                    }
                    scope.Complete();
                    return RedirectToAction("Index");
                }

            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var model = db.SanPham.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: ves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var scope = new TransactionScope())
            {
                var model = db.SanPham.Find(id);
                db.SanPham.Remove(model);
                db.SaveChanges();
                var path = Server.MapPath(PICTURE_PATH);
                System.IO.File.Delete(path + model.MaSanPham);
                scope.Complete();
                return RedirectToAction("Index");
            }
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