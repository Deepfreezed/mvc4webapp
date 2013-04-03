using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{	
	public class Course
	{
		public Course()
		{
			Sections = new List<CourseSection>();
		}

		public string Department { get; set; }
		public string CourseID { get; set; }
		public string CourseName { get; set; }		
		public string Credits { get; set; }		
		public int Size { get; set; }
		public int Enrolled { get; set; }
		public string Status { get; set; }
		public string AdditionalNotes { get; set; }
		public List<CourseSection> Sections { get; set; }
	}
}