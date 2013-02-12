using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class AssignmentsController : Controller
    {
        //
		// GET: /Assignments/Assignment1

		public ActionResult Assignment1()
        {
            return View();
        }

		//
		// GET: /Assignments/AcademicHonestyPolicy

		public ActionResult AcademicHonestyPolicy()
		{
			return View();
		}

		//
		// GET: /Assignments/StudentConductProcess

		public ActionResult StudentConductProcess()
		{
			return View();
		}
    }
}
