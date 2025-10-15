using HumanResourceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResourceManagement.Controllers
{
    public class DashBController : Controller
    {
        HRMEntities db = new HRMEntities();
        // GET: DashB
        public ActionResult Index()
        {
            return View();
        }
    }
}