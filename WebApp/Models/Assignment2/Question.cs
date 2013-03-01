using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Assignment2
{
	public class Question
	{
		public Question()
		{
			Answer = -1;
		}

		public int FirstNumber { get; set; }
		public int SecondNumber { get; set; }
		public int Answer { get; set; }
		
		public int Solution 
		{ 
			get
			{
				return FirstNumber + SecondNumber;
			}
		}
	}
}