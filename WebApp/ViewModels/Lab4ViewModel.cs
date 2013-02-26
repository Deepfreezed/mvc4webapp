using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.SessionState;
using WebApp.Helpers;
using Raven.Client;
using Raven.Json.Linq;
using WebApp.Models;

namespace WebApp.ViewModels
{
	public class Lab4ViewModel
	{
		/// <summary>
		/// Gets or sets the calculated value.
		/// </summary>
		/// <value>
		/// The calculated value.
		/// </value>
		[Required]
		[Display(Name = "Value")]
		[RegularExpression(@"[0-9]+", ErrorMessage = "{0} must be a Number.")]
		public string CalculatedValue { get; set; }

		public string SessionTimeOut { get; set; } 
		public LimitedQueue<string> RunningCalculations { get; set; }
		public decimal RunningTotal { get; set; }


	}
}