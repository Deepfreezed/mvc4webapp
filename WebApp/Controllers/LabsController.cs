using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Helpers;
using WebApp.ViewModels;
using Omu.ValueInjecter;

namespace WebApp.Controllers
{
	public class LabsController : RavenController
    {
		//
		// POST: /Labs/LabMain
		public ActionResult LabMain()
		{
			return View();
		}

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
			Lab4DataAccess dataAccess = new Lab4DataAccess(RavenSession);
			PrintingCalculator item = dataAccess.RetrieveFromSession();
			Lab4ViewModel model = new Lab4ViewModel();

			if(item != null)
			{
				model.InjectFrom(item);
			}

			return View("Lab4", model);
		}
		
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Lab4(string add, string subtract, string multiply, string divide, string clearmemory, string execute, Lab4ViewModel model)
		{			
			int value = 0;
			Lab4DataAccess dataAccess = new Lab4DataAccess(RavenSession);
			PrintingCalculator item = dataAccess.RetrieveFromSession();
			
			if(!string.IsNullOrEmpty(model.CalculatedValue) && int.TryParse(model.CalculatedValue, out value))
			{
				if(!string.IsNullOrEmpty(add))
				{
					item.Add(value);
				}
				else if(!string.IsNullOrEmpty(subtract))
				{
					item.Subtract(value);
				}
				else if(!string.IsNullOrEmpty(multiply))
				{
					item.Multiply(value);
				}
				else if(!string.IsNullOrEmpty(divide))
				{
					item.Divide(value);
				}

				//Save to store
				RavenSession.Store(item);
				RavenSession.SaveChanges();

				//Map changes to model
				model.InjectFrom(item);
			}
			else
			{
				ModelState.Clear();

				if(!string.IsNullOrEmpty(clearmemory))
				{
					dataAccess.ClearFromSession();

					//Clear Model
					model = new Lab4ViewModel();
				}
				//else if(!string.IsNullOrEmpty(execute))
				//{
				//    lab4Model.Execute();
				//}
			}

			ModelState.Remove("CalculatedValue");
			model.CalculatedValue = string.Empty;

			return View(model);
		}
    }
}
