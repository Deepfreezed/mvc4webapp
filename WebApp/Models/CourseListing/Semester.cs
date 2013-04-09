using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.CourseListing
{
	public class Semester
	{
		public Semester()
		{
			Departments = new List<Department>();
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public List<Department> Departments { get; set; }
	}
}