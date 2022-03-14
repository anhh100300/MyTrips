using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;


namespace WebApplication.Controllers
{
    public class ShoppingCartController : Controller
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
        // GET: ShoppingCart
        public ActionResult Index()
        {
            GetShoppingCart();
            var hashtable = new Hashtable();
            foreach (var chiTietDonHang in ShoppingCart)
            {
                if (hashtable[chiTietDonHang.SanPham1.MaSanPham] != null)
                {
                    (hashtable[chiTietDonHang.SanPham1.MaSanPham] as ChiTietDonHang).SoLuong += chiTietDonHang.SoLuong;
                }
                else hashtable[chiTietDonHang.SanPham1.MaSanPham] = chiTietDonHang;
            }
            ShoppingCart.Clear();
            foreach (ChiTietDonHang chiTietDonHang in hashtable.Values)
                ShoppingCart.Add(chiTietDonHang);
            return View(ShoppingCart);
        }
        // GET: ShoppingCart/Details/5
        // GET: ShoppingCart/Create
        [HttpPost]
        public ActionResult Create(int productId, int quantity)
        {
            GetShoppingCart();
            var product = db.SanPham.Find(productId);
            ShoppingCart.Add(new ChiTietDonHang
            {
                SanPham1 = product,
                SoLuong = quantity
            });
            return RedirectToAction("Index");
        }
        // GET: ShoppingCart/Edit/5
        [HttpPost]
        public ActionResult Edit(int[] product_id, int[] quantity)
        {
            GetShoppingCart();
            ShoppingCart.Clear();
            if (product_id != null)
                for (int i = 0; i < product_id.Length; i++)
                    if (quantity[i] > 0)
                    {
                        var product = db.SanPham.Find(product_id[i]);
                        ShoppingCart.Add(new ChiTietDonHang
                        {
                            SanPham1 = product,
                            SoLuong = quantity[i]
                        });
                    }
            Session["ShoppingCart"] = ShoppingCart;
            return RedirectToAction("Index");
        }
        // GET: ShoppingCart/Delete/5
        public ActionResult Delete()
        {
            GetShoppingCart();
            ShoppingCart.Clear();
            Session["ShoppingCart"] = ShoppingCart;
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
