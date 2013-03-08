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
	public class Lab5ViewModel
	{
		public int ID { get; set; }

		/// <summary>
		/// Gets or sets the Stock Symbol value.
		/// </summary>
		/// <value>
		/// The Stock Symbol value.
		/// </value>
		[Required]
		[Display(Name = "Stock Symbol")]
		[StringLength(20, ErrorMessage = "{0} must be between {2} and {1} characters long.", MinimumLength = 1)]
		//[RegularExpression(@"([A-Z]{1,4})", ErrorMessage = "Not in correct format for a Stock Symbol.")]
		public string StockSymbol { get; set; }

		public string StockSummary { get; set; }

		public string StockChart { get; set; }

		public DateTime LastUpdated { get; set; }

		private int m_RefreshInterval = 1;

		public int RefreshInterval
		{
			get { return m_RefreshInterval; }
			set { m_RefreshInterval = value; }
		}

	}
}