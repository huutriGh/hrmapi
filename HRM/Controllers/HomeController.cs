using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace HRM.Controllers
{
    public class HomeController : Controller
    {
        // GET: api/Home
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
            
        }

    }
}
