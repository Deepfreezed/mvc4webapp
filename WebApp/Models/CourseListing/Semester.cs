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
			DepartmentIds = new List<string>();
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public List<string> DepartmentIds { get; set; }
	}
}