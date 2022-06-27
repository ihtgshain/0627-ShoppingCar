using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using prjShoppingCar.Models;

namespace prjShoppingCar.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        dbShoppingCarEntities db = new dbShoppingCarEntities();
        // GET: Member
        public ActionResult Index()
        {
            var products = db.tProduct.OrderByDescending(m => m.fId).ToList();
            return View("../Home/Index","_LayoutMember",products);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login","Home");
        }

        public ActionResult ShoppingCar()
        {
            string fUserId = User.Identity.Name;
            var orderDetails=db.tOrderDetail.Where(x=>x.fUserId==fUserId && x.fIsApproved=="否").ToList();
            return View(orderDetails);
        }
        [HttpPost]
        public ActionResult ShoppingCar(string fReceiver, string fEmail, string fAddress) 
        { 
            string fUserId=User.Identity.Name;
            string guid=Guid.NewGuid().ToString();
            tOrder order = new tOrder();
            order.fOrderGuid = guid;
            order.fUserId = fUserId;
            order.fReceiver = fReceiver;
            order.fEmail = fEmail;
            order.fAddress = fAddress;
            order.fDate = DateTime.Now;
            db.tOrder.Add(order);

            var carList=db.tOrderDetail.Where(x=>x.fIsApproved =="否" && x.fUserId==fUserId).ToList();
            foreach(var item in carList)
            {
                item.fOrderGuid = guid;
                item.fIsApproved = "是";
            }
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }

        public ActionResult AddCar(string fPId)
        {
            string fUserId=User.Identity.Name;

            var currentCar = db.tOrderDetail.FirstOrDefault(x => x.fPId == fPId && x.fIsApproved == "否" && x.fUserId == fUserId);
            if (currentCar == null)
            {
                var product = db.tProduct.FirstOrDefault(x => x.fPId == fPId);
                tOrderDetail orderDetail = new tOrderDetail();
                orderDetail.fUserId = fUserId;
                orderDetail.fPId = product.fPId;
                orderDetail.fName = product.fName;
                orderDetail.fPrice = product.fPrice;
                orderDetail.fQty = 1;
                orderDetail.fIsApproved = "否";
                db.tOrderDetail.Add(orderDetail);
            }
            else
                currentCar.fQty += 1;
            db.SaveChanges();
            return RedirectToAction("ShoppingCar");
        }

        public ActionResult DeletCar(int fId)
        {
            var orderDetail=db.tOrderDetail.FirstOrDefault(x=>x.fId == fId);
            db.tOrderDetail.Remove(orderDetail);
            db.SaveChanges();
            return RedirectToAction("ShoppingCar");
        }

        public ActionResult OrderList()
        {
            string fUserId = User.Identity.Name;
            var orders = db.tOrder.Where(x => x.fUserId == fUserId).OrderByDescending(x => x.fDate).ToList();
            return View(orders);
        }

        public ActionResult OrderDetail(string fOrderGuid)
        {
            var orderDetails = db.tOrderDetail.Where(x => x.fOrderGuid == fOrderGuid).ToList();
            return View(orderDetails);
        }
    }
}