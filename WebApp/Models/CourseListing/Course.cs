using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebApp.Models.CourseListing
{	
	public class Course
	{		
		public Course()
		{
			Sections = new List<Section>();			
		}

		public string Id { get; set; }
		public string CourseNumber { get; set; }
		public string DepartmentID { get; set; }
		public string SemesterID { get; set; }

		public string CourseID 
		{ 
			get
			{
				return string.Format("{0}{1}", DepartmentID, CourseNumber);
			}
		}

		public string CourseName { get; set; }		
		public string Credits { get; set; }		
		public List<Section> Sections { get; set; }
		public List<string> Prerequisites { get; set; }
		public Rating CourseRating { get; set; }
		public int TotalEnrolment { get; set; }
		public int TotalSize { get; set; }
		
		public int HistoricalEnrolment 
		{ 
			get
			{
				int historicalEnrolment = 0;
				int totalEnrolment = TotalEnrolment; //Sections.Sum(s => s.Enrolled)
				int totalSize = TotalSize; //Sections.Sum(s => s.Size) + 

				if(totalEnrolment > 0 && totalSize > 0)
				{
					historicalEnrolment = (int)Math.Round((decimal)(((decimal)totalEnrolment / (decimal)totalSize) * 100), 0);
				}
				

				return historicalEnrolment;
			}
		}

		/// <summary>
		/// Adds the enrolment statics.
		/// </summary>
		/// <param name="enrolled">The enrolled.</param>
		/// <param name="size">The size.</param>
		public void AddEnrolmentStatics(int enrolled, int size)
		{
			TotalEnrolment = TotalEnrolment + enrolled;
			TotalSize = TotalSize + size;
		}
	}
		
	public enum Rating
	{		
		NORMAL = 0,
		HOT = 1,
		HOTTEST = 2
	}
}