using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.CourseListing
{
	public class Section
	{
		public Section()
		{
			Days = new List<string>();
			Time = new List<string>();
			Dates = new List<string>();
			Room = new List<string>();
			Instructor = new List<string>();
		}

		public string Id { get; set; }
		public string SectionID { get; set; }
		public string GradeMethod { get; set; }
		public List<string> Days { get; set; }
		public List<string> Time { get; set; }
		public List<string> Dates { get; set; }
		public List<string> Room { get; set; }
		public List<string> Instructor { get; set; }
		public int Size { get; set; }
		public int Enrolled { get; set; }
		public string Status { get; set; }
	}
}