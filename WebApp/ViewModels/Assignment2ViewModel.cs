using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApp.ViewModels
{
	public class Assignment2ViewModel
	{
		[Required]
		[Display(Name = "Answer")]
		[RegularExpression(@"[0-9]+", ErrorMessage = "{0} must be a Number.")]
		public string Answer { get; set; }
				
		public int FirstNumber { get; set; }
		public int SecondNumber { get; set; }
		public int Solution { get; set; }
		public bool AnswereVerified { get; set; }
		public bool Done { get; set; }
		public int ProblemsCorrect { get; set; }
		public int ProblemsAnswered { get; set; }
		public int ProblemsWrong { get; set; }
		public string Rating { get; set; }

		public string RatingMessage 
		{ 
			get
			{
				string msg = string.Empty;

				decimal ratingPercent = (Convert.ToDecimal(ProblemsCorrect) / Convert.ToDecimal(ProblemsAnswered)) * 100;

				if(ratingPercent > 95)
				{
					msg = "Awesome !!!";
				}
				else if(ratingPercent > 85)
				{
					msg = "Almost perfect...";
				}
				else if(ratingPercent > 80)
				{
					msg = "Doing great, keep it up !!!";
				}
				else if(ratingPercent > 70)
				{
					msg = "Good job, keep improving...";
				}
				else if(ratingPercent > 60)
				{
					msg = "Practice makes perfect...";
				}
				else if(ratingPercent > 50)
				{
					msg = "Consentrate...";
				}
				else if(ratingPercent > 40)
				{
					msg = "You need to keep practicing...";
				}
				else if(ratingPercent > 20)
				{
					msg = "Keep going...";
				}

				return msg;
			}
		}
	}
}