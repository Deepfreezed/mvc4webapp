using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Models.CourseListing;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebApp.ViewModels
{
	public class CourseListingViewModel
	{
		public CourseListingViewModel()
		{
			Courses = new List<Course>();
		}

		[Required]
		[Display(Name = "Semester")]
		public string Semester { get; set; }

		[Required]
		[Display(Name = "Department")]
		public string Department { get; set; }

		public List<Course> Courses { get; set; }

		public List<Semester> Semesters { get; set; }
		public List<Department> Departments { get; set; }
	}
}
