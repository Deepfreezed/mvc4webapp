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
				if(!string.IsNullOrEmpty(UserID))
				{
					return DigitSum % 100;
				}
				else
				{
					return 0;
				}				 
			}
		}

		public int DigitSum
		{
			get
			{
				if(!string.IsNullOrEmpty(UserID))
				{
					return UserID.Sum(c => c - '0');
				}
				else
				{
					return 0;
				}				
			}
		}
	}
}