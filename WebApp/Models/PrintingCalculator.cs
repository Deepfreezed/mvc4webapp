using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
	public class PrintingCalculator
	{
		private decimal m_RunningTotal;

		public PrintingCalculator()
		{
			RunningCalculations = new LimitedQueue<string>();
			RunningCalculations.Limit = 20;
			RunningTotal = 0;
		}

		public int ID { get; set; }
		public LimitedQueue<string> RunningCalculations { get; set; }
		public string SessionTimeOut { get; set; }

		public decimal RunningTotal
		{
			get { return Math.Round(m_RunningTotal, 2); }
			set { m_RunningTotal = value; }
		}

		/// <summary>
		/// Adds the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Add(int value)
		{
			RunningTotal = RunningTotal + value;

			RunningCalculations.Enqueue(value.ToString() + "+");
		}

		/// <summary>
		/// Subtracts the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Subtract(int value)
		{
			RunningTotal = RunningTotal - value;

			RunningCalculations.Enqueue(value.ToString() + "-");
		}

		/// <summary>
		/// Multiplies the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Multiply(int value)
		{
			RunningTotal = RunningTotal * value;

			RunningCalculations.Enqueue(value.ToString() + "*");
		}

		/// <summary>
		/// Divides the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Divide(int value)
		{
			RunningTotal = RunningTotal / value;

			RunningCalculations.Enqueue(value.ToString() + "/");
		}
	}
}
