using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Helpers;
using WebApp.ViewModels;
using Omu.ValueInjecter;
using System.Net;
using System.IO;
using System.Net.Cache;

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
		//[ValidateAntiForgeryToken]
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

				ModelState.Remove("CalculatedValue");
				model.CalculatedValue = string.Empty;
			}
			else
			{
				if(!string.IsNullOrEmpty(clearmemory))
				{
					ModelState.Clear();
					ModelState.Remove("CalculatedValue");
					model.CalculatedValue = string.Empty;

					dataAccess.ClearFromSession();

					//Clear Model
					model = new Lab4ViewModel();
				}
				else if(!string.IsNullOrEmpty(execute))
				{
					ModelState.Clear();
					ModelState.Remove("CalculatedValue");
					model.CalculatedValue = string.Empty;
				}
				else
				{
					//Map changes to model
					model.InjectFrom(item);
				}
			}

			return View("Lab4", model);
		}

		//
		// POST: /Labs/Lab5
		public ActionResult Lab5()
		{
			Lab5DataAccess dataAccess = new Lab5DataAccess(RavenSession);
			Lab5ViewModel model = dataAccess.RetrieveFromSession();

			return View("Lab5", model);
		}

		[HttpPost]
		[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		public ActionResult Lab5(Lab5ViewModel model)
		{
			string chartSrc = string.Format("http://ichart.finance.yahoo.com/{0}={1}", "b?s", model.StockSymbol);
			string url = string.Format("http://finance.yahoo.com/q/cp?s={0}&ql=1", model.StockSymbol);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "GET";
			httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
        
			//httpWebRequest.ContentType = "application/x-www-form-urlencoded";

			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			StreamReader streamReader = new StreamReader(responseStream);
			string response = streamReader.ReadToEnd();

			string start = @"<div class=""rtq_leaf"">";
			string end = @"<div id=""yfi_toolbox_mini_rtq"">";

			int startIndex = response.IndexOf(start);
			int endIndex = response.IndexOf(end);

			if(startIndex > 0 && endIndex > 0 && (endIndex > startIndex))
			{
				string stockSummaryHTML = response.Substring(startIndex, endIndex - startIndex);
				stockSummaryHTML = stockSummaryHTML + @"</div></div></div></div>";

				model.StockSummary = stockSummaryHTML;
				model.StockChart = chartSrc;
				model.LastUpdated = DateTime.Now;

				//Save the information
				Lab5DataAccess dataAccess = new Lab5DataAccess(RavenSession);
				Lab5ViewModel lab5Model = dataAccess.RetrieveFromSession();
				lab5Model.InjectFrom(model);
			}
			else
			{
				using(StringWriter sw = new StringWriter())
				{
					ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "_StockSummaryPartial");
					ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
					viewResult.View.Render(viewContext, sw);

					model.StockSummary = sw.GetStringBuilder().ToString();
				}
			}

			return View("Lab5", model);
		}

		//
		// POST: /Labs/Lab6
		public ActionResult Lab6()
		{			
			return View("Lab6", null);
		}
    }
}
