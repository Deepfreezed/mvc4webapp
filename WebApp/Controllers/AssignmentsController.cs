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
		public ActionResult Assignment3(string id)
		{
			Assignment3ViewModel viewModel = new Assignment3ViewModel();

			if(!string.IsNullOrEmpty(id))
			{
				viewModel.AirportCode = id.ToUpper();

				string url = string.Format("http://www.airport-data.com/airport/{0}/weather.html", id);
				string start = @"<h2>Current Condition</h2>";
				string end = @"<a name=""forecast"">";
				string response = CommonFunctions.MakeHttpWebRequest(url, start, end);

				if(!string.IsNullOrEmpty(response))
				{
					viewModel.AirportWeatherHTML = response;
				}

				string url2 = string.Format("http://www.airport-data.com/airport/{0}/", id);
				string start2 = @"<h2>Location & QuickFacts</h2>";
				string end2 = @"</section>";
				string response2 = CommonFunctions.MakeHttpWebRequest(url2, start2, end2);

				if(!string.IsNullOrEmpty(response2))
				{
					viewModel.AirportInformationHTML = response2;
				}
			}			

			return View("Assignment3", viewModel);
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Assignment3(Assignment3ViewModel viewModel)
		{
			//if(!string.IsNullOrEmpty(viewModel.AirportLocation))
			//{
			//    string url = string.Format("http://www.airport-data.com/usa-airports/search.php?field=location&kw={0}", viewModel.AirportLocation);
			//    string start = @"<table class=""table table-bordered"">";
			//    string end = @"</table>";
			//    string response = CommonFunctions.MakeHttpWebRequest(url, start, end);

			//    if(!string.IsNullOrEmpty(response))
			//    {
			//        viewModel.AirportsNearLocationHTML = response + "</table>";

			//        //Replace string
			//        viewModel.AirportsNearLocationHTML = viewModel.AirportsNearLocationHTML.Replace(@"http://www.airport-data.com/airport/", "/Assignments/Assignment3/");
			//    }
			//}
			if(!string.IsNullOrEmpty(viewModel.State))
			{
				string url = string.Format("http://www.airport-data.com/usa-airports/state/Minnesota.html", viewModel.SelectedState);
				string start = @"<table class=""table"" id=""tbl_airports"">";
				string end = @"</table>";
				string response = string.Empty;

				response = CommonFunctions.MakeHttpWebRequest("www.google.com", start, end);

				response = CommonFunctions.MakeHttpWebRequest("www.airport-data.com/usa-airports/state/Minnesota.html", start, end);

				response = CommonFunctions.MakeHttpWebRequest(url, start, end);
				
				if(!string.IsNullOrEmpty(response))
				{
					viewModel.AirportsNearLocationHTML = response + "</table>";

					//Replace string
					viewModel.AirportsNearLocationHTML = viewModel.AirportsNearLocationHTML.Replace(@"http://www.airport-data.com/airport/", "/Assignments/Assignment3/");
				}
			}			

			return View("Assignment3", viewModel);
		}
    }
}
