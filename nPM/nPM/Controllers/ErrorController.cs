using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nPM.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult Index()
        {
            return PartialView("IndexError");
        }

    }
}
