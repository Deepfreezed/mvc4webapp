using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.SessionState;

namespace WebApp.Models
{
	public class Lab4Model
	{
		private const string SessionKey = "Lab4Model";

		public Lab4Model()
		{
			RunningCalculations = new LimitedQueue<string>(20);
			RunningTotal = 0;
		}

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

		public string SessionTimeOut
		{
			get
			{
				return HttpContext.Current.Session.GetSessionTimeOut(SessionKey);
			}			
		}

		/// <summary>
		/// Gets or sets the running calculations.
		/// </summary>
		/// <value>
		/// The running calculations.
		/// </value>
		public LimitedQueue<string> RunningCalculations { get; set; }

		/// <summary>
		/// Gets or sets the running total.
		/// </summary>
		/// <value>
		/// The running total.
		/// </value>
		public decimal RunningTotal { get; set; }

		/// <summary>
		/// Adds the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Add(int value)
		{
			RunningTotal = RunningTotal + value;

			RunningCalculations.Enqueue("+" + value.ToString());
		}

		/// <summary>
		/// Subtracts the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Subtract(int value)
		{
			RunningTotal = RunningTotal - value;

			RunningCalculations.Enqueue("-" + value.ToString());
		}

		/// <summary>
		/// Multiplies the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Multiply(int value)
		{
			RunningTotal = RunningTotal * value;

			RunningCalculations.Enqueue("X" + value.ToString());
		}

		/// <summary>
		/// Divides the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Divide(int value)
		{
			RunningTotal = RunningTotal / value;

			RunningCalculations.Enqueue("/" + value.ToString());
		}

		/// <summary>
		/// Clears the memory.
		/// </summary>
		public void ClearMemory()
		{
			ClearFromSession();
		}

		/// <summary>
		/// Executes this instance.
		/// </summary>
		public void Execute()
		{
			
		}

		/// <summary>
		/// Gets the running calculations.
		/// </summary>
		/// <returns></returns>
		public string GetRunningCalculations()
		{
			string msg = string.Empty;

			foreach(string str in RunningCalculations)
			{
				msg = msg + str;
			}

			return msg;
		}

		/// <summary>
		/// Retrieve from session.
		/// </summary>
		/// <returns></returns>
		public static Lab4Model RetrieveFromSession()
		{
			Lab4Model lab4Model;

			if(HttpContext.Current.Session.GetWithTimeout(SessionKey) != null)
			{
				lab4Model = (Lab4Model)HttpContext.Current.Session.GetWithTimeout(SessionKey);
			}
			else
			{
				//initialize a link list with max 20 operations
				lab4Model = new Lab4Model();

				//Create session with a 10 min timeout
				TimeSpan expireAfter = new TimeSpan(0, 10, 0);
				HttpContext.Current.Session.AddWithTimeout(SessionKey, lab4Model, expireAfter);
			}

			return lab4Model;
		}

		/// <summary>
		/// Clears from session.
		/// </summary>
		public static void ClearFromSession()
		{
			HttpContext.Current.Session.Remove(SessionKey);
		}
	}

	public class LimitedQueue<T> : Queue<T>
	{
		private int limit = -1;

		public int Limit
		{
			get { return limit; }
			set { limit = value; }
		}

		public LimitedQueue(int limit)
			: base(limit)
		{
			this.Limit = limit;
		}

		public new void Enqueue(T item)
		{
			if(this.Count >= this.Limit)
			{
				this.Dequeue();
			}
			base.Enqueue(item);
		}
	}
}