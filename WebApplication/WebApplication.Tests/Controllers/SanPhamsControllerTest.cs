using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Controllers;
using WebApplication.Models;
using Moq;
using System.Transactions;

namespace WebApplication.Tests.Controllers
{
    [TestClass]
    public class SanPhamsControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            var controller = new SanPhamsController();

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<SanPham>;
            Assert.IsNotNull(model);

            var db = new CsK24_MyTripEntities();
            Assert.AreEqual(db.SanPham.Count(), model.Count);
        }
        [TestMethod]
        public void TestIndex2()
        {
            var controller = new SanPhamsController();

            var result = controller.Index2() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<SanPham>;
            Assert.IsNotNull(model);

            var db = new CsK24_MyTripEntities();
            Assert.AreEqual(db.SanPham.Count(), model.Count);
        }
        [TestMethod]
        public void TestCreateG()
        {
            var controller = new SanPhamsController();

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void TestCreateP()
        {
            var rand = new Random();
            var sanpham = new SanPham
            {

                ThongTinSanPham = rand.NextDouble().ToString(),
                NoiDung = rand.NextDouble().ToString(),
                GiaTien = -rand.Next(),
                KhoiHanh = DateTime.Now

            };

            var controller = new SanPhamsController();
            var result0 = controller.Create(sanpham, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Giá vé nhỏ hơn 0 ! Vui lòng nhập lại ", controller.ModelState["GiaTien"].Errors[0].ErrorMessage);


            sanpham.GiaTien = -sanpham.GiaTien;
            controller.ModelState.Clear();

            result0 = controller.Create(sanpham, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Picture not found!", controller.ModelState[""].Errors[0].ErrorMessage);

            var picture = new Mock<HttpPostedFileBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
            new System.Web.Routing.RouteData(), controller);

            var fileName = String.Empty;
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => s);
            picture.Setup(p => p.SaveAs(It.IsAny<string>())).Callback<string>(s => fileName = s);

            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result1 = controller.Create(sanpham, picture.Object) as RedirectToRouteResult;
                Assert.IsNotNull(result1);
                Assert.AreEqual("Index", result1.RouteValues["action"]);

                var db = new CsK24_MyTripEntities();
                var entity = db.SanPham.SingleOrDefault(p => p.ThongTinSanPham == sanpham.ThongTinSanPham);
                Assert.IsNotNull(entity);
                Assert.AreEqual(sanpham.GiaTien, entity.GiaTien);

                Assert.IsTrue(fileName.StartsWith("~/Upload/Product/"));
                Assert.IsTrue(fileName.EndsWith(entity.MaSanPham.ToString()));
            }
        }
        [TestMethod]
        public void TestEditG()
        {
            var controller = new SanPhamsController();
            var result0 = controller.Edit(0) as HttpNotFoundResult;
            Assert.IsNotNull(result0);

            var db = new CsK24_MyTripEntities();
            var sanpham = db.SanPham.First();
            var result1 = controller.Edit(sanpham.MaSanPham) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as SanPham;
            Assert.IsNotNull(model);
            Assert.AreEqual(sanpham.GiaTien, model.GiaTien);
            Assert.AreEqual(sanpham.ThongTinSanPham, model.ThongTinSanPham);
            Assert.AreEqual(sanpham.NoiDung, model.NoiDung);
            Assert.AreEqual(sanpham.KhoiHanh, model.KhoiHanh);
        }
        [TestMethod]
        public void TestEditP()
        {
            var rand = new Random();
            var db = new CsK24_MyTripEntities();
            var sanpham = db.SanPham.AsNoTracking().First();
            sanpham.ThongTinSanPham = rand.NextDouble().ToString();
            sanpham.NoiDung = rand.NextDouble().ToString();
            sanpham.GiaTien = -rand.Next();

            var controller = new SanPhamsController();

            var result0 = controller.Edit(sanpham, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Giá vé nhỏ hơn 0 ! Vui lòng nhập lại ", controller.ModelState["GiaTien"].Errors[0].ErrorMessage);

            var picture = new Mock<HttpPostedFileBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var fileName = String.Empty;
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => s);
            picture.Setup(p => p.SaveAs(It.IsAny<string>())).Callback<string>(s => fileName = s);

            using (var scope = new TransactionScope())
            {
                sanpham.GiaTien = -sanpham.GiaTien;
                controller.ModelState.Clear();
                var result1 = controller.Edit(sanpham, picture.Object) as RedirectToRouteResult;
                Assert.IsNotNull(result1);
                Assert.AreEqual("Index", result1.RouteValues["action"]);

                var entity = db.SanPham.Find(sanpham.MaSanPham);
                Assert.IsNotNull(entity);
                Assert.AreEqual(sanpham.GiaTien, entity.GiaTien);
                Assert.AreEqual(sanpham.ThongTinSanPham, entity.ThongTinSanPham);
                Assert.AreEqual(sanpham.KhoiHanh, entity.KhoiHanh);

                Assert.AreEqual("~/Upload/Product/" + sanpham.MaSanPham, fileName);
                //Assert.IsTrue(fileName.StartsWith("~/Upload/Product/"));
                //Assert.IsTrue(fileName.EndsWith(entity.MaVe.ToString()));
            }
        }
        [TestMethod]
        public void TestDeleteG()
        {
            var controller = new SanPhamsController();
            var result0 = controller.Delete(0) as HttpNotFoundResult;
            Assert.IsNotNull(result0);

            var db = new CsK24_MyTripEntities();
            var sanpham = db.SanPham.First();
            var result1 = controller.Delete(sanpham.MaSanPham) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as SanPham;
            Assert.IsNotNull(model);
            Assert.AreEqual(sanpham.GiaTien, model.GiaTien);
            Assert.AreEqual(sanpham.ThongTinSanPham, model.ThongTinSanPham);
            Assert.AreEqual(sanpham.NoiDung, model.NoiDung);
            Assert.AreEqual(sanpham.KhoiHanh, model.KhoiHanh);
        }
        [TestMethod]
        public void TestDeleteP()
        {
            var db = new CsK24_MyTripEntities();
            var sanpham = db.SanPham.AsNoTracking().First();

            var controller = new SanPhamsController();

            var context = new Mock<HttpContextBase>();
            var server = new Mock<HttpServerUtilityBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var filePath = String.Empty;
            var tempDir = System.IO.Path.GetTempFileName();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s =>
            {
                filePath = s;
                return tempDir;
            });

            using (var scope = new TransactionScope())
            {
                System.IO.File.Delete(tempDir);
                System.IO.Directory.CreateDirectory(tempDir);
                tempDir = tempDir + "/";
                System.IO.File.Create(tempDir + sanpham.MaSanPham).Close();
                Assert.IsTrue(System.IO.File.Exists(tempDir + sanpham.MaSanPham));

                var result = controller.DeleteConfirmed(sanpham.MaSanPham) as RedirectToRouteResult;
                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var entity = db.SanPham.Find(sanpham.MaSanPham);
                Assert.IsNull(entity);

                Assert.AreEqual("~/UpLoad/HinhAnh/", filePath);
                Assert.IsFalse(System.IO.File.Exists(tempDir + sanpham.MaSanPham));
            }
        }
        [TestMethod]
        public void TestPicture()
        {
            var controller = new SanPhamsController();

            var context = new Mock<HttpContextBase>();
            var server = new Mock<HttpServerUtilityBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var filePath = String.Empty;
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => filePath = s);

            var result = controller.Picture(0) as FilePathResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Upload/Product/0", result.FileName);
            Assert.AreEqual("images", result.ContentType);
        }
        [TestMethod]
        public void TestDispose()
        {
            using (var controller = new SanPhamsController()) { }
        }
        [TestMethod]
        public void TestDetails()
        {
            var controller = new SanPhamsController();
            var result0 = controller.Details(0) as HttpNotFoundResult;
            Assert.IsNotNull(result0);

            var db = new CsK24_MyTripEntities();
            var sanpham = db.SanPham.First();
            var result1 = controller.Details(sanpham.MaSanPham) as ViewResult;
            Assert.IsNotNull(result1);

            var model = result1.Model as SanPham;
            Assert.IsNotNull(model);
            Assert.AreEqual(sanpham.GiaTien, model.GiaTien);
            Assert.AreEqual(sanpham.ThongTinSanPham, model.ThongTinSanPham);
            Assert.AreEqual(sanpham.NoiDung, model.NoiDung);
            Assert.AreEqual(sanpham.KhoiHanh, model.KhoiHanh);
        }
        [TestMethod]
        public void TestSearch()
        {
            var db = new CsK24_MyTripEntities();
            var sanpham = db.SanPham.ToList();
            var keyword = sanpham.First().ThongTinSanPham.Split().First();
            sanpham = sanpham.Where(p => p.ThongTinSanPham.ToLower().Contains(keyword.ToLower())).ToList();

            var controller = new SanPhamsController();
            var result = controller.Search(keyword) as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<SanPham>;
            Assert.IsNotNull(model);

            Assert.AreEqual("Index2", result.ViewName);
            Assert.AreEqual(sanpham.Count(), model.Count);
            Assert.AreEqual(keyword, result.ViewData["Keyword"]);
        }
    }
}
