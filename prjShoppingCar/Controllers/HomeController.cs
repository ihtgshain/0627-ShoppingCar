using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prjShoppingCar.Models;
using System.Web.Security; 

namespace prjShoppingCar.Controllers
{
    public class HomeController : Controller
    {
        dbShoppingCarEntities db = new dbShoppingCarEntities();
        
        // GET: Home
        public ActionResult Index()
        {
            var products = db.tProduct.OrderByDescending(x=>x.fId).ToList();
            return View(products);
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string fUserId,string fPwd)
        {
            var member = db.tMember.FirstOrDefault(m => m.fUserId == fUserId && m.fPwd == fPwd);
            if(member== null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }
            Session["Welcome"] = member.fName + "歡迎光臨";
            FormsAuthentication.RedirectFromLoginPage(fUserId, true);
            return RedirectToAction("Index", "Member");
            
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(tMember pMember)
        {
            if(ModelState.IsValid==false)
                return View();

            var member=db.tMember.FirstOrDefault(m=>m.fUserId == pMember.fUserId);
            if (member == null)
            {
                db.tMember.Add(pMember);
                db.SaveChanges();
                return RedirectToAction("Login");
            }

            ViewBag.Message = "此帳號已有人使用，註冊失敗";
            return View();
        }
    }
}