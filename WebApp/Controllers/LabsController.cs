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

		//
		// POST: /Labs/Lab4
		public ActionResult Lab4()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Lab4(string add, string subtract, string multiply, string divide, Lab4Model model)
		{
			int value = 0;
			Lab4Model lab4Model = Lab4Model.RetrieveFromSession();

			if(!string.IsNullOrEmpty(model.CalculatedValue) && int.TryParse(model.CalculatedValue, out value))
			{
				if(!string.IsNullOrEmpty(add))
				{
					lab4Model.Add(value);
				}
				else if(!string.IsNullOrEmpty(subtract))
				{
					lab4Model.Subtract(value);
				}
				else if(!string.IsNullOrEmpty(multiply))
				{
					lab4Model.Multiply(value);
				}
				else if(!string.IsNullOrEmpty(divide))
				{
					lab4Model.Divide(value);
				}
			}
			else
			{
				
				
			}		

			return View(lab4Model);
		}

		//[HttpPost]
		//[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		//[ValidateInput(false)]
		//public ActionResult Lab4(string clearmemory, string execute)
		//{
		//    Lab4Model lab4Model = Lab4Model.RetrieveFromSession();

		//    if(!string.IsNullOrEmpty(clearmemory))
		//    {
		//        lab4Model.ClearMemory();
		//    }
		//    else if(!string.IsNullOrEmpty(execute))
		//    {
		//        lab4Model.Execute();
		//    }

		//    return View(lab4Model);
		//}
    }
}
