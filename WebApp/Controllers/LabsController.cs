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
using RestSharp;
using HtmlAgilityPack;

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

		public ActionResult Lab7()
		{
			Lab7ViewModel model = new Lab7ViewModel();
			RestClientSettings settings = new RestClientSettings();

			settings.URL = "http://www3.mnsu.edu/courses/selectform.asp";
			settings.Method = Method.POST;

			//add valid header for kicks although they are not looking for it
			settings.Parameters.Add(new Parameter() { Name = "Accept", Value = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", Type = ParameterType.HttpHeader });
			settings.Parameters.Add(new Parameter() { Name = "Accept-Encoding", Value = "gzip, deflate", Type = ParameterType.HttpHeader });
			settings.Parameters.Add(new Parameter() { Name = "Accept-Language", Value = "en-US,en;q=0.5", Type = ParameterType.HttpHeader });
			settings.Parameters.Add(new Parameter() { Name = "Host", Value = "www3.mnsu.edu", Type = ParameterType.HttpHeader });
			settings.Parameters.Add(new Parameter() { Name = "Referer", Value = "http://www3.mnsu.edu/courses/", Type = ParameterType.HttpHeader });
			settings.Parameters.Add(new Parameter() { Name = "User-Agent", Value = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0", Type = ParameterType.HttpHeader });

			// adds to POST or URL query string based on Method
			settings.Parameters.Add(new Parameter() { Name = "All", Value = "All Sections", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "campus", Value = "1,2,3,4,5,6,7,9,A,B,C,I,L,M,N,P,Q,R,S,T,W,U,V,X,Z", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "college", Value = "", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "courseid", Value = "", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "courselevel", Value = "", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "coursenum", Value = "", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "days", Value = "ALL", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "endTime", Value = "2359", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "semester", Value = "20143Fall 2013", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "startTime", Value = "0600", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "subject", Value = "CS", Type = ParameterType.GetOrPost });

			string content = CommonFunctions.MakeRestSharpRequest(settings);

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(content);

			HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//table[2]/tr");

			Course course;
			List<Course> courses = new List<Course>();
			foreach(HtmlNode node in htmlNodes)
			{
				if(node.NodeType == HtmlNodeType.Element && node.HasAttributes && node.Attributes["bgcolor"].Value == "#DFE4FF")
				{
					course = new Course();

					string[] value = node.InnerText.Trim().Replace("&nbsp;", string.Empty).Split(new string[] { "\r\n\t\t\t" }, StringSplitOptions.RemoveEmptyEntries);

					if(value != null && value.Length == 3)
					{
						//Department
						course.Department = value[0].Trim().Replace(@"&nbsp", string.Empty);

						//courseID and Name
						string[] courseIDandName = value[1].Split(new string[] { "&#150;" }, StringSplitOptions.None);
						course.CourseID = courseIDandName[0].Trim();
						course.CourseName = courseIDandName[1].Trim();

						//Number of credits
						course.Credits = value[2].Replace("(", string.Empty).Replace(" Credits)", string.Empty).Trim();
					}

					courses.Add(course);
				}
			}

			model.Courses = courses;

			return View("Lab7", model);
		}
	}
}
