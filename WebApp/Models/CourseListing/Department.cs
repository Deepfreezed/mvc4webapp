using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.CourseListing
{
	public class Department
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public List<Course> Courses { get; set; }
	}
}