using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using WebApp.Models.Mortgage;

namespace WebApp.ViewModels
{
	public class MortgageCalculatorViewModel
	{
		public MortgageCalculatorViewModel()
		{
			Action = MortgagePageAction.ShowLogIn;
		}

		[Required]
		[Display(Name = "Mortgage Name")]
		public string MortgageName { get; set; }

		[Required]
		[Display(Name = "Loan Amount")]
		public double LoanAmount { get; set; }

		[Required]
		[Display(Name = "Annual Interest Rate %")]
		public double AnnualInterestRate { get; set; }

		[Required]
		[Display(Name = "Original Loan Term years")]
		public double OrgLoanTerm { get; set; }
				
		[Display(Name = "Target Loan Term years")]
		public double TargetLoanTerm { get; set; }

		[Display(Name = "ExtraMonthlyPayment")]
		public double ExtraMonthlyPayment { get; set; }

		//[Display(Name = "Extra Yearly Payment")]
		//public double ExtraYearlyPayment { get; set; }

		[Required]
		[Display(Name = "Down Payment")]
		public double DownPayment { get; set; }

		[Display(Name = "User Name")]
		public string UserName { get; set; }

		public MortgageUser User { get; set; }

		public MortgagePageAction Action { get; set; }

		public string ShowMortgageID { get; set; }

		public string[] CompareMortgageIDs { get; set; }
	}

	public enum MortgagePageAction
	{
		ShowLogIn,
		ShowNoUserFound,
		ShowUserMortgages,
		ShowMortgageCalculations,
		ShowMortgageAmortization,
		ShowMortgageCompare,
		CreateNewUser,		
		CreateNewMortgage
	}

}