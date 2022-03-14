using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using WebApplication.Models;
using WebApplication.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Moq;
using System.Transactions;
using System.Collections;
using System.Web.Routing;

namespace WebApplication.Tests.Controllers
{
    public class MockHttpSession : HttpSessionStateBase
    {
        public Hashtable Buffer = new Hashtable();
        public override object this[string key]
        {

            get
            {
                return Buffer[key];
            }
            set
            {
                Buffer[key] = value;
            }
        }
    }
    [TestClass]
    public class ShoppingCartControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            var session = new MockHttpSession();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Session).Returns(session);

            var controller = new ShoppingCartController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            session["ShoppingCart"] = null;
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<ChiTietDonHang>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);

            var db = new CsK24_MyTripEntities();
            var sanpham = db.SanPham.First();
            var shoppingcart = new List<ChiTietDonHang>();
            shoppingcart.Add(new ChiTietDonHang
            {
                SanPham1 = sanpham,
                SoLuong = 1
            });
            var chitietdonhang = new ChiTietDonHang();
            chitietdonhang.SanPham1 = sanpham;
            chitietdonhang.SoLuong = 2;
            shoppingcart.Add(chitietdonhang);

            session["ShoppingCart"] = shoppingcart;
            result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);

            model = result.Model as List<ChiTietDonHang>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual(sanpham.MaSanPham, model.First().SanPham1.MaSanPham);
            Assert.AreEqual(3, model.First().SoLuong);

        }

        [TestMethod]
        public void TestCreate()
        {
            var session = new MockHttpSession();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Session).Returns(session);

            var controller = new ShoppingCartController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var db = new CsK24_MyTripEntities();
            var sanpham = db.SanPham.First();
            var result = controller.Create(sanpham.MaSanPham, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);

            var shoppingCart = session["ShoppingCart"] as List<ChiTietDonHang>;
            Assert.IsNotNull(shoppingCart);
            Assert.AreEqual(1, shoppingCart.Count);
            Assert.AreEqual(sanpham.MaSanPham, shoppingCart.First().SanPham1.MaSanPham);
            Assert.AreEqual(2, shoppingCart.First().SoLuong);
        }
        [TestMethod]
        public void TestDispose()
        {
            using (var controller = new ShoppingCartController()) { }
        }
    }
}
