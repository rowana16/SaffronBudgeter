﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Saffron.Controllers
{
    public class PartialController : Controller
    {
        // GET: Partial
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            ViewBag.TestText = "Hello World";
            return PartialView("_PartialTest");

        }
    }
}