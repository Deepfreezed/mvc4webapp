using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class LabModels
	{
		[Required]
		[Display(Name = "User name")]
		[StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
		public string UserName { get; set; }

		[Required]
		[Display(Name = "User ID")]
		[RegularExpression(@"[0-9]+", ErrorMessage = "{0} must be a Number.")]
		public string UserID { get; set; }

		public int ModulusSum 
		{ 
			get
			{
				return DigitSum % 100;
			}
		}

		public int DigitSum
		{
			get
			{
				return UserID.Sum(c => c - '0');
			}
		}
	}
}