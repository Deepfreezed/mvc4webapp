using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class LabsController : Controller
    {
		//
		// POST: /Labs/Lab3
		public ActionResult Lab3()
        {
            return View();
        }

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Lab3(LabModels model)
		{
			if(string.IsNullOrEmpty(model.UserID) && string.IsNullOrEmpty(model.UserName))
			{
				return View();
			}
			else
			{
				return View(model);
			}

			
			//return PartialView("_ModulusSumPartial", model);
		}
    }
}
