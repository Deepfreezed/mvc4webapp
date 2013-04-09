using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Models.CourseListing;

namespace WebApp.ViewModels
{
	public class Assignment4ViewModel
	{
		public Assignment4ViewModel()
		{
			Courses = new List<Course>();
		}

		public List<Course> Courses { get; set; }
	}
}