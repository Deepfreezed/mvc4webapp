using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
	public class Course
	{
		public string Department { get; set; }
		public string CourseID { get; set; }
		public string CourseName { get; set; }
		public string GradeMethod { get; set; }
		public string Credits { get; set; }
		public string Days { get; set; }
		public string Time { get; set; }
		public string Dates { get; set; }
		public string Room { get; set; }
		public string Instructor { get; set; }
		public int Size { get; set; }
		public int Enrolled { get; set; }
		public string Status { get; set; }
		public string AdditionalNotes { get; set; }
	}
}