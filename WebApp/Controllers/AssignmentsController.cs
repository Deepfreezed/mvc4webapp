using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models.Assignment2;
using WebApp.Helpers;
using WebApp.ViewModels;
using Omu.ValueInjecter;

namespace WebApp.Controllers
{
    public class AssignmentsController : RavenController
    {
		//
		// GET: /Assignments/AssignmentMain

		public ActionResult AssignmentMain()
		{
			return View();
		}

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

		//
		// GET: /Assignments/Assignment2

		public ActionResult Assignment2()
		{
			Assignment2DataAccess dataAccess = new Assignment2DataAccess(RavenSession);
			MathPractice exam = dataAccess.RetrieveFromSession();
			Question currentQuestion = exam.CurrentQuestion;
			Assignment2ViewModel viewModel = new Assignment2ViewModel();

			if(currentQuestion != null)
			{
				viewModel.InjectFrom(exam, currentQuestion);
				viewModel.Answer = string.Empty;
			}
			else
			{
				viewModel.InjectFrom(exam);
				viewModel.Done = true;
			}

			return View("Assignment2", viewModel);
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Assignment2(string submitanswer, string answereverified, string newpractise, Assignment2ViewModel viewModel)
		{
			Assignment2DataAccess dataAccess = new Assignment2DataAccess(RavenSession);
			MathPractice exam = dataAccess.RetrieveFromSession();
			Question currentQuestion = exam.CurrentQuestion;

			if(exam != null && currentQuestion == null && string.IsNullOrEmpty(newpractise))
			{
				ModelState.Clear();
				viewModel.InjectFrom(exam);
				viewModel.Done = true;
			}
			else if(!string.IsNullOrEmpty(submitanswer))
			{
				int result;
				int.TryParse(viewModel.Answer, out result);
				currentQuestion.Answer = result;

				if(currentQuestion.Answer == currentQuestion.Solution)
				{
					currentQuestion = exam.CurrentQuestion;

					if(currentQuestion != null)
					{
						viewModel.InjectFrom(exam, currentQuestion);
						ModelState.Remove("Answer");
						viewModel.Answer = string.Empty;
						viewModel.AnswereVerified = true;
					}
					else
					{
						ModelState.Clear();
						viewModel.InjectFrom(exam);
						viewModel.Done = true;
					}
				}
				else
				{
					viewModel.InjectFrom(exam, currentQuestion);
					viewModel.AnswereVerified = false;
				}
			}
			else if(!string.IsNullOrEmpty(answereverified))
			{
				currentQuestion = exam.CurrentQuestion;

				if(currentQuestion != null)
				{
					viewModel.InjectFrom(exam, currentQuestion);
					ModelState.Remove("Answer");
					viewModel.Answer = string.Empty;
					viewModel.AnswereVerified = true;
				}
				else
				{
					ModelState.Clear();
					viewModel.InjectFrom(exam);
					viewModel.Done = true;
				}
			}
			else if(!string.IsNullOrEmpty(newpractise))
			{
				ModelState.Clear();
				exam = dataAccess.CreateNewSession();
				currentQuestion = exam.CurrentQuestion;
				viewModel.InjectFrom(exam, currentQuestion);
				viewModel.Answer = string.Empty;
			}

			return View("Assignment2", viewModel);
		}

		//
		// GET: /Assignments/Assignment3
		public ActionResult Assignment3(string id, string UseProxy)
		{
			Assignment3ViewModel viewModel = new Assignment3ViewModel();

			if(!string.IsNullOrEmpty(id))
			{
				viewModel.AirportCode = id.ToUpper();

				string url = string.Format("http://www.airport-data.com/airport/{0}/weather.html", id);
				string start = @"<h2>Current Condition</h2>";
				string end = @"<a name=""forecast"">";
				string response = Assignment3MakeHttpWebRequest(viewModel, url, start, end);

				if(!string.IsNullOrEmpty(response))
				{
					viewModel.AirportWeatherHTML = response;
				}

				string url2 = string.Format("http://www.airport-data.com/airport/{0}/", id);
				string start2 = @"<h2>Location & QuickFacts</h2>";
				string end2 = @"</section>";
				string response2 = Assignment3MakeHttpWebRequest(viewModel, url2, start2, end2);

				if(!string.IsNullOrEmpty(response2))
				{
					viewModel.AirportInformationHTML = response2;
				}
			}
			else
			{				
				//Check if a proxy is setup in cookies
				string proxyIP = CommonFunctions.ReadCookie("ProxyIP");
				int proxyPort;
				int.TryParse(CommonFunctions.ReadCookie("ProxyPort"), out proxyPort);

				viewModel.ProxyIP = proxyIP;
				viewModel.ProxyPort = proxyPort;
			}

			return View("Assignment3", viewModel);
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Assignment3(Assignment3ViewModel viewModel, string UseProxy)
		{
			if(!string.IsNullOrEmpty(viewModel.State))
			{
				string url = string.Format("http://airport-data.com/usa-airports/state/{0}.html", viewModel.SelectedState);
				string start = @"<table class=""table"" id=""tbl_airports"">";
				string end = @"</table>";

				string response = Assignment3MakeHttpWebRequest(viewModel, url, start, end);						
				
				if(!string.IsNullOrEmpty(response))
				{
					viewModel.AirportsNearLocationHTML = response + "</table>";

					//Replace string
					viewModel.AirportsNearLocationHTML = viewModel.AirportsNearLocationHTML.Replace(@"http://www.airport-data.com/airport/", "/Assignments/Assignment3/");
				}
			}			

			return View("Assignment3", viewModel);
		}

		/// <summary>
		/// Assignment3s the make HTTP web request.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		/// <param name="url">The URL.</param>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <returns></returns>
		private static string Assignment3MakeHttpWebRequest(Assignment3ViewModel viewModel, string url, string start, string end)
		{
			string response = string.Empty;

			if(!string.IsNullOrEmpty(viewModel.ProxyIP) && viewModel.ProxyPort > 0)
			{
				response = CommonFunctions.MakeHttpWebRequest(url, start, end, viewModel.ProxyIP, viewModel.ProxyPort);

				//Store values in cookies
				CommonFunctions.StoreCookie("ProxyIP", viewModel.ProxyIP);
				CommonFunctions.StoreCookie("ProxyPort", viewModel.ProxyPort);
			}
			else
			{
				response = CommonFunctions.MakeHttpWebRequest(url, start, end);
			}

			return response;
		}
    }
};
