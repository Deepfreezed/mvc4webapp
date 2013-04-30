using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.CourseListing
{
	public class Department
	{
		public Department()
		{
			CourseIds = new List<string>();
		}

		public string Id { get; set; }
		public string DepartmentID { get; set; }
		public string SemesterID { get; set; }
		public string Name { get; set; }
		public List<string> CourseIds { get; set; }
	}
}