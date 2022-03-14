using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class DonHangsController : Controller
    {
        private CsK24_MyTripEntities db = new CsK24_MyTripEntities();
        private List<ChiTietDonHang> ShoppingCart = null;
        private void GetShoppingCart()
        {
            if (Session["ShoppingCart"] != null)
                ShoppingCart = Session["ShoppingCart"] as List<ChiTietDonHang>;
            else
            {
                ShoppingCart = new List<ChiTietDonHang>();
                Session["ShoppingCart"] = ShoppingCart;
            }
        }

        [Authorize(Roles = "phongvo@2728")]
        public ActionResult Index()
        {
            var model = db.DonHang.ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            GetShoppingCart();
            ViewBag.Cart = ShoppingCart;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DonHang model)
        {
            ValidateBill(model);
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    model.NgayBan = DateTime.Now;
                    db.DonHang.Add(model);
                    db.SaveChanges();
                    foreach (var item in ShoppingCart)
                    {
                        db.ChiTietDonHang.Add(new ChiTietDonHang
                        {
                            MaSanPham = model.SoHoaDon,
                            SanPham = item.SanPham1.MaSanPham,
                            SoLuong = item.SoLuong,
                            GiaTien = item.SanPham1.GiaTien
                        });
                    }
                    db.SaveChanges();

                    scope.Complete();
                    Session["ShoppingCart"] = null;
                    return RedirectToAction("Index2", "SanPhams");
                }
            }
            GetShoppingCart();
            ViewBag.Cart = ShoppingCart;
            return View(model);
        }

        private void ValidateBill(DonHang model)
        {
            var regex = new Regex("[0-9]{3}");
            GetShoppingCart();
            if (ShoppingCart.Count == 0)
                ModelState.AddModelError("", "There is no item in shopping cart!");
            if (!regex.IsMatch(model.SoDienThoai))
                ModelState.AddModelError("SoDienThoai", "Wrong phone number");
        }

        // GET: DonHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHang.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        // POST: DonHangs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SoHoaDon,NgayBan,HovaTen,SoDienThoai,DiaChi,PheDuyet")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(donHang);
        }

        // GET: DonHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonHang donHang = db.DonHang.Find(id);
            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        // POST: DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DonHang donHang = db.DonHang.Find(id);
            db.DonHang.Remove(donHang);
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
