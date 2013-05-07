using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models.Assignment2;
using WebApp.Helpers;
using WebApp.ViewModels;
using Omu.ValueInjecter;
using WebApp.Models.CourseListing;
using Raven.Imports.Newtonsoft.Json;
using WebApp.Models.Mortgage;

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
			//Initialize Data Model
			Assignment2DataAccess dataAccess = new Assignment2DataAccess(RavenSession);

			//Retrieve quiz from the database
			MathPractice exam = dataAccess.RetrieveFromSession();
			Question currentQuestion = exam.CurrentQuestion;

			//Initialize View Model
			Assignment2ViewModel viewModel = new Assignment2ViewModel();

			if(currentQuestion != null)
			{
				//Use Object mapper to map Data Model to View Model
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

		/// <summary>
		/// Assignment3s the specified view model.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		/// <param name="UseProxy">The use proxy.</param>
		/// <returns></returns>
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
		/// Assignment4s the specified refresh data.
		/// </summary>
		/// <param name="refreshData">The refresh data.</param>
		/// <param name="reloadData">The reload data.</param>
		/// <param name="recalculateStats">The recalculate stats.</param>
		/// <returns></returns>
		public ActionResult Assignment4(string reloadData, string recalculateStats, string deleteData)
		{
			CourseListingDataAccess dataAccess = new CourseListingDataAccess(RavenSession);
			Assignment4ViewModel model = new Assignment4ViewModel();
			
			if(!string.IsNullOrEmpty(reloadData) && reloadData.Trim() == "1")
			{				
				dataAccess.LoadAllCourseDataToDatabase();
			}

			if(!string.IsNullOrEmpty(deleteData) && deleteData.Trim() == "1")
			{
				dataAccess.DeleteAllCourseData();
			}

			if(!string.IsNullOrEmpty(recalculateStats) && recalculateStats.Trim() == "1")
			{
				dataAccess.PopulateHistoricalData();
			}

			model.Courses = dataAccess.GetCoursesBySemesterIDandDepartmentID("20143Fall 2013", "IT");

			return View("Assignment4", model);
		}

		#region Assignment 5
		/// <summary>
		/// Assignment5s this instance.
		/// </summary>
		/// <returns></returns>
		public ActionResult Assignment5()
		{
			CourseListingDataAccess dataAccess = new CourseListingDataAccess(RavenSession);
			CourseListingViewModel viewModel = new CourseListingViewModel();
			
			//Populate the dropdowns
			viewModel.Semesters = dataAccess.GetAllSemesters();
			viewModel.Departments = dataAccess.GetAllDepartmentsForCurrentSemester(); //TODO: AJAX populate based off semester

			return View("Assignment5", viewModel);
		}

		/// <summary>
		/// Assignment5s the specified view model.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous]
		public ActionResult Assignment5(CourseListingViewModel viewModel)
		{
			CourseListingDataAccess dataAccess = new CourseListingDataAccess(RavenSession);

			//Populate the dropdowns
			viewModel.Semesters = dataAccess.GetAllSemesters();
			viewModel.Departments = dataAccess.GetAllDepartmentsForCurrentSemester(); //TODO: AJAX populate based off semester

			if(!string.IsNullOrEmpty(viewModel.Semester) && !string.IsNullOrEmpty(viewModel.Department))
			{
				viewModel.Courses = dataAccess.GetCoursesBySemesterIDandDepartmentID(viewModel.Semester, viewModel.Department);
			}
			
			return View("Assignment5", viewModel);
		}

		/// <summary>
		/// APIs the json.
		/// </summary>
		/// <param name="actionModel">The action model.</param>
		/// <param name="semesterID">The semester ID.</param>
		/// <param name="departmentID">The department ID.</param>
		/// <param name="courseID">The course ID.</param>
		/// <returns></returns>
		[ParsePath]
		public ActionResult ApiJson(string semesterID, string departmentID, string courseID)
		{
			if(!string.IsNullOrEmpty(semesterID) && !string.IsNullOrEmpty(departmentID))
			{
				List<Course> courses = new List<Course>();
				CourseListingDataAccess dataAccess = new CourseListingDataAccess(RavenSession);
				
				if (string.IsNullOrEmpty(courseID))
				{
					courses = dataAccess.GetCoursesBySemesterIDandDepartmentID(semesterID, departmentID);
				}
				else
				{
					courses = dataAccess.GetCourses(semesterID, departmentID, courseID);
				}
				

				MvcJsonResult jsonNetResult = new MvcJsonResult();
				jsonNetResult.Formatting = Formatting.Indented;
				jsonNetResult.Data = courses;

				return jsonNetResult;
			}
			else
			{
				return RedirectToAction("ApiHelp", "Assignments");
			}			
		}

		/// <summary>
		/// APIs the XML.
		/// </summary>
		/// <param name="actionModel">The action model.</param>
		/// <param name="semesterID">The semester ID.</param>
		/// <param name="departmentID">The department ID.</param>
		/// <param name="courseID">The course ID.</param>
		/// <returns></returns>
		[ParsePath]
		public ActionResult ApiXml(string semesterID, string departmentID, string courseID)
		{
			if(!string.IsNullOrEmpty(semesterID) && !string.IsNullOrEmpty(departmentID))
			{
				List<Course> courses = new List<Course>();
				CourseListingDataAccess dataAccess = new CourseListingDataAccess(RavenSession);

				if(string.IsNullOrEmpty(courseID))
				{
					courses = dataAccess.GetCoursesBySemesterIDandDepartmentID(semesterID, departmentID);
				}
				else
				{
					courses = dataAccess.GetCourses(semesterID, departmentID, courseID);
				}

				return new MvcXmlResult(courses);
			}
			else
			{
				return RedirectToAction("ApiHelp", "Assignments");
			}	
		}

		/// <summary>
		/// Reloads the data.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
		public ActionResult LoadData(string semesterID)
		{
			CourseListingDataAccess dataAccess = new CourseListingDataAccess(RavenSession);

			if(!string.IsNullOrEmpty(semesterID))
			{
				HttpContext.Server.ScriptTimeout = 600000;
				dataAccess.LoadSemesterToDatabase(semesterID);
			}
			else
			{
				HttpContext.Server.ScriptTimeout = 6000000;
				dataAccess.LoadAllCourseDataToDatabase();
			}			

			return RedirectToAction("Assignment5", "Assignments");
		}

		/// <summary>
		/// Refresh the stats.
		/// </summary>
		/// <returns></returns>
		public ActionResult RefreshStats()
		{
			CourseListingDataAccess dataAccess = new CourseListingDataAccess(RavenSession);
			dataAccess.PopulateHistoricalData();

			return RedirectToAction("Assignment5", "Assignments");
		}

		/// <summary>
		/// Deletes the data.
		/// </summary>
		/// <returns></returns>
		public ActionResult DeleteData()
		{
			CourseListingDataAccess dataAccess = new CourseListingDataAccess(RavenSession);

			dataAccess.DeleteAllCourseData();

			return RedirectToAction("Assignment5", "Assignments");
		}

		/// <summary>
		/// APIs the help.
		/// </summary>
		/// <returns></returns>
		public ActionResult ApiHelp()
		{
			return View("ApiHelp");
		}
		#endregion

		/// <summary>
		/// Mortgages the calculator.
		/// </summary>
		/// <returns></returns>
		public ActionResult MortgageCalculator()
		{
			MortgageCalculatorDataAccess dataAccess = new MortgageCalculatorDataAccess(RavenSession);
			MortgageUser mortgageUser = dataAccess.RetrieveFromSession();
			MortgageCalculatorViewModel model = new MortgageCalculatorViewModel();
			model.User = mortgageUser;

			if(mortgageUser != null && !string.IsNullOrEmpty(mortgageUser.UserName) && mortgageUser.Mortgages.Count == 0)
			{
				model.Action = MortgagePageAction.CreateNewMortgage;
			}
			else if(mortgageUser != null && !string.IsNullOrEmpty(mortgageUser.UserName) && mortgageUser.Mortgages.Count > 0)
			{
				model.Action = MortgagePageAction.ShowUserMortgages;
			}

			return View("MortgageCalculator", model);
		}	

		/// <summary>
		/// Mortgages the calculator.
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous]
		public ActionResult MortgageCalculator(string action, string userlogin, string[] compareIDs, MortgageCalculatorViewModel model)
		{
			MortgageCalculatorDataAccess dataAccess = new MortgageCalculatorDataAccess(RavenSession);
			MortgageUser mortgageUser = null;
			MortgagePageAction actionPage = MortgagePageAction.ShowLogIn;
			string value = string.Empty;

			if(!string.IsNullOrEmpty(action) && action.Contains('_'))
			{
				string[] actionValue = action.Split('_');

				action = actionValue[0];
				value = actionValue[1];
			}

			switch(action)
			{
				case "getuser":
					if(!string.IsNullOrEmpty(userlogin))
					{
						mortgageUser = dataAccess.GetUser(userlogin);

						if(mortgageUser == null || string.IsNullOrEmpty(mortgageUser.Id) || string.IsNullOrEmpty(mortgageUser.UserName))
						{
							actionPage = MortgagePageAction.ShowNoUserFound;
						}
						else
						{
							dataAccess.CreateSessions(mortgageUser);
							actionPage = MortgagePageAction.ShowUserMortgages;
						}
					}
					break;
				case "createuser":
					actionPage = MortgagePageAction.CreateNewUser;
					break;
				case "saveuser":
					mortgageUser = new MortgageUser();
					mortgageUser.InjectFrom(model);
					dataAccess.SaveUser(mortgageUser);
					actionPage = MortgagePageAction.CreateNewMortgage;
					break;
				case "createmortgage":
					mortgageUser = dataAccess.RetrieveFromSession();
					actionPage = MortgagePageAction.CreateNewMortgage;
					break;
				case "savemortgage":
					mortgageUser = dataAccess.RetrieveFromSession();				
					MortgageInformation mortgageInfo = new MortgageInformation();
					mortgageInfo.InjectFrom(model);
					mortgageUser.Mortgages.Add(mortgageInfo);
					dataAccess.SaveUser(mortgageUser);
					actionPage = MortgagePageAction.ShowUserMortgages;
					break;				
				case "signout":
					dataAccess.SignOutSession();
					actionPage = MortgagePageAction.ShowLogIn;
					mortgageUser = null;
					Session.Abandon();
					break;
				case "deleteuser":
					dataAccess.DeleteCurrentUser();
					actionPage = MortgagePageAction.ShowLogIn;
					mortgageUser = null;
					break;
				case "calculatemortgage":
					mortgageUser = dataAccess.RetrieveFromSession();
					if(!string.IsNullOrEmpty(value))
					{
						model.ShowMortgageID = value;
						actionPage = MortgagePageAction.ShowMortgageCalculations;
					}
					break;
				case "deletemortgage":
					mortgageUser = dataAccess.RetrieveFromSession();
					if(!string.IsNullOrEmpty(value))
					{
						MortgageInformation deletedMortgage = mortgageUser.GetMortgageByName(value);
						mortgageUser.Mortgages.Remove(deletedMortgage);
						dataAccess.SaveChanges();
						actionPage = MortgagePageAction.ShowUserMortgages;
					}
					break;
				case "amortizemortgage":
					mortgageUser = dataAccess.RetrieveFromSession();
					if(!string.IsNullOrEmpty(value))
					{
						model.ShowMortgageID = value;
						actionPage = MortgagePageAction.ShowMortgageAmortization;
					}
					break;
				case "comparemortgage":
					mortgageUser = dataAccess.RetrieveFromSession();
					if(compareIDs != null && compareIDs.Length > 0)
					{
						model.CompareMortgageIDs = compareIDs;
						actionPage = MortgagePageAction.ShowMortgageCompare;
					}
					break;
				default:
					break;
			}

			model.Action = actionPage;
			model.User = mortgageUser;

			return View("MortgageCalculator", model);
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
}
