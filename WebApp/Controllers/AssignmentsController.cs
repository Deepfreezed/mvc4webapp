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
    }
}
