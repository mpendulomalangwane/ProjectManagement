using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nPM.Controllers
{
    public class LoginController : _BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Authenticate(string Username, string Password)
        {

            if (ModelState.IsValid)
            {
                //authenticate user
                var user = UnitOfWork.RepositoryUser.GetSpecific(u => new { u.Id, u.FullName }, w => w.UserName == Username && w.PassWord == Password).FirstOrDefault();
                if (user != null)
                {
                    HttpCookie myCookie = new HttpCookie("UserSettings");
                    myCookie["UserId"] = user.Id.ToString();
                    myCookie.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(myCookie);

                    Response.Cookies["udetails"].Value = user.FullName.ToString();
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Login");
            }
            return View("Index");
        }

    }
}
