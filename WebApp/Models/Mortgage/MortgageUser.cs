using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Mortgage
{
	public class MortgageUser
	{
		public MortgageUser()
		{
			Mortgages = new List<MortgageInformation>();
		}

		public string Id { get; set; }

		public string UserName { get; set; }

		public List<MortgageInformation> Mortgages { get; set; }

		/// <summary>
		/// Gets the name of the mortgage by.
		/// </summary>
		/// <param name="mortgageName">Name of the mortgage.</param>
		/// <returns></returns>
		public MortgageInformation GetMortgageByName(string mortgageName)
		{
			MortgageInformation mortgagteInfo = Mortgages.FirstOrDefault(m => m.MortgageName.Equals(mortgageName, StringComparison.InvariantCultureIgnoreCase));

			return mortgagteInfo;
		}
	}

	public class MortgageInformation
	{
		public string MortgageName { get; set; }

		public double LoanAmount { get; set; }

		public double AnnualInterestRate { get; set; }

		public double OrgLoanTerm { get; set; }

		public double TargetLoanTerm { get; set; }

		public double ExtraMonthlyPayment { get; set; }

		//public double ExtraYearlyPayment { get; set; }

		public double DownPayment { get; set; }

		public double OriginalMonthlyPayment()
		{
			double monthlyInterest = (double)((AnnualInterestRate / 12) / 100);
			double paymentMonths = OrgLoanTerm * 12;
			double variable = Math.Pow(1 + monthlyInterest, paymentMonths);
			double paymentRemaining = LoanAmount - DownPayment;

			return Math.Round((paymentRemaining * ((monthlyInterest * variable) / (variable - 1))), 2);
		}

		public double TargetMonthlyPayment()
		{
			double monthlyInterest = ((AnnualInterestRate / 12) / 100);
			double paymentMonths = (TargetLoanTerm * 12);
			double variable = Math.Pow(1 + monthlyInterest, paymentMonths);
			double paymentRemaining = (LoanAmount - DownPayment);

			return Math.Round((paymentRemaining * ((monthlyInterest * variable) / (variable - 1))), 2);
		}

		public double OriginalInterestPayment()
		{
			double orgPaymentMonths = (OrgLoanTerm * 12);
			double originalMonthlyPayment = OriginalMonthlyPayment();
			double totalPayment = originalMonthlyPayment * orgPaymentMonths;
			double paymentRemaining = LoanAmount - DownPayment;

			return totalPayment - paymentRemaining;
		}

		public double TargetInterestSaving()
		{
			double orgPaymentMonths = (OrgLoanTerm * 12);
			double trgPaymentMonths = (TargetLoanTerm * 12);
			double originalMonthlyPayment = OriginalMonthlyPayment();
			double targetMonthlyPayment = TargetMonthlyPayment();

			double originalPayment = originalMonthlyPayment * orgPaymentMonths;
			double targetPayment = targetMonthlyPayment * trgPaymentMonths;

			return originalPayment - targetPayment;
		}

		public double TargetAdditionalPayment()
		{
			return TargetMonthlyPayment() - OriginalMonthlyPayment();
		}

		public int ExtraPaymentMonthsSaved()
		{
			double earlyPaymentMonths = EarlyPaymentMonths();
			int orgPayOffMonths = (int)Math.Round(OrgLoanTerm * 12);
			int newPayOffMonths = (int)Math.Round(earlyPaymentMonths);

			return orgPayOffMonths - newPayOffMonths;
		}

		public double ExtraPaymentInterestSaved()
		{
			double orgMonthlyPayment = OriginalMonthlyPayment();
			double orgPaymentMonths = (OrgLoanTerm * 12);
			double extraMonthlyPayment = ExtraMonthlyPaymentTotal();
			double extraPaymentMonths = EarlyPaymentMonths();

			
			double originalPayment = orgMonthlyPayment * orgPaymentMonths;
			double targetPayment = extraMonthlyPayment * extraPaymentMonths;

			return Math.Round(originalPayment - targetPayment, 2);
		}

		public double ExtraMonthlyPaymentTotal()
		{
			return OriginalMonthlyPayment() + ExtraMonthlyPayment;			
		}

		/// <summary>
		/// Earlies the payment months.
		/// </summary>
		/// <returns></returns>
		public double EarlyPaymentMonths()
		{
			double P = LoanAmount - DownPayment;
			double i = ((AnnualInterestRate / 12) / 100);
			//double A = ExtraMonthlyPaymentTotal();
			double months = 0;

			//double nn = Math.Log(A / (A - (P * i))) / Math.Log(1 + i);

			int n = 1;
			while((P > 0) && (!double.IsNaN(P)))
			{
				P = P - EarlyPaymentPrincipalAtMonth(n);
				double A = ExtraMonthlyPaymentTotal();

				n++;
				months++;

				if(P < A)
				{
					months = months + (P / A);
					break;
				}
			}

			return months;
		}

		/// <summary>
		/// Earlies the payment principal at month.
		/// </summary>
		/// <param name="month">The month.</param>
		/// <returns></returns>
		public double EarlyPaymentPrincipalAtMonth(double month)
		{
			double n = month;
			double P = LoanAmount - DownPayment;
			double i = ((AnnualInterestRate / 12) / 100);
			double A = ExtraMonthlyPaymentTotal();

			double PPn = A * (Math.Pow((1 + i), (n - 1))) - ((P * i) * (Math.Pow((1 + i), (n - 1))));

			return PPn;
		}

		/// <summary>
		/// Principals the remaining by month.
		/// </summary>
		/// <param name="month">The month.</param>
		/// <returns></returns>
		public double PrincipalRemainingByMonth(double month)
		{
			double PV = LoanAmount - DownPayment;
			double P = ExtraMonthlyPaymentTotal();
			double r = ((AnnualInterestRate / 12) / 100);
			double n = month;

			double remaining = (PV * Math.Pow((1 + r), n)) - (P * ((Math.Pow((1 + r), n) - 1) / r));

			return remaining;
		}

		/// <summary>
		/// Earlies the payment interest at month.
		/// </summary>
		/// <param name="month">The month.</param>
		/// <returns></returns>
		public double EarlyPaymentInterestAtMonth(double month)
		{
			return ExtraMonthlyPaymentTotal() - EarlyPaymentPrincipalAtMonth(month);
		}

		public List<double> EarlyPaymentPrincipalByMonth()
		{
			List<double> principals = new List<double>();
			int months = (int)Math.Round(EarlyPaymentMonths(), 0);

			for(int i = 0; months >= i; i++)
			{
				double principal = EarlyPaymentPrincipalAtMonth(i);
				principals.Add(principal);
			}

			return principals;
		}

		public List<double> PrincipalRemainingByMonth()
		{
			List<double> principals = new List<double>();
			int months = (int)Math.Round(EarlyPaymentMonths(), 0);
			double principalRemaining = (LoanAmount - DownPayment);

			for(int i = 1; months >= i; i++)
			{
				principalRemaining = principalRemaining - EarlyPaymentPrincipalAtMonth(i);
				principals.Add(principalRemaining);
			}

			return principals;
		}

		public double EarlyPaymentTotalInterest()
		{
			double months = EarlyPaymentMonths();
			int intmonths = (int)months;
			double interest = 0;
			double monthRemainder = months - intmonths;

			for(int i = 0; intmonths >= i; i++)
			{
				interest = interest + EarlyPaymentInterestAtMonth(i);
			}

			if(monthRemainder > 0)
			{
				interest = interest + EarlyPaymentInterestAtMonth(monthRemainder);
			}

			return interest;
		}
	}
}