using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Models;
using RestSharp;
using HtmlAgilityPack;
using WebApp.Models.CourseListing;
using Raven.Client;

namespace WebApp.Helpers
{
	public class CourseListingDataAccess
	{
		private IDocumentSession m_RavenSession;
		
		public CourseListingDataAccess(IDocumentSession RavenSession)
		{
			m_RavenSession = RavenSession;
			LoadAllCourseDataToDatabase();
		}

		public Semester GetSemesterByID(string semesterID)
		{
			Semester item = new Semester();

			if(!string.IsNullOrEmpty(semesterID))
			{
				item = m_RavenSession.Load<Semester>(semesterID);
			}			

			return item;
		}

		/// <summary>
		/// Gets the courses by department ID.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <param name="departmentID">The department ID.</param>
		/// <returns></returns>
		public List<Course> GetCoursesByDepartmentID(string semesterID, string departmentID)
		{
			List<Course> courses = new List<Course>();

			if(!string.IsNullOrEmpty(semesterID) && !string.IsNullOrEmpty(departmentID))
			{
				Semester semester = m_RavenSession.Load<Semester>(semesterID);
				Department department = semester.Departments.FirstOrDefault(d => d.Id.Equals(departmentID, StringComparison.InvariantCultureIgnoreCase));

				if(department != null && department.Courses.Count > 0)
				{
					courses = department.Courses;
				}					
			}

			return courses;
		}

		/// <summary>
		/// Loads all course data to database.
		/// </summary>
		public void LoadAllCourseDataToDatabase()
		{
			Semester semester = m_RavenSession.Load<Semester>("20143Fall 2013");

			if(semester == null || semester.Departments.Count == 0)
			{
				string contenet = RetrieveRawCourseData("20143Fall 2013");
				semester = ParseRawCourseData(contenet);
				semester.Id = "20143Fall 2013";
				semester.Name = "Fall 2013";

				m_RavenSession.Store(semester);
				m_RavenSession.SaveChanges();
			}
			//TODO:
		}

		#region Private Methods
		

		/// <summary>
		/// Parses the raw course data.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns></returns>
		private Semester ParseRawCourseData(string content)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(content);

			HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//table[2]/tr");

			Semester semester = new Semester();
			Department department = null;
			Course course = null;
			Section section = null;
			List<Course> courses = new List<Course>();
			bool readAdditionalInfo = false;
			bool addNewCourse = false;

			foreach(HtmlNode node in htmlNodes)
			{
				if(node.NodeType == HtmlNodeType.Element && node.HasAttributes && node.Attributes["bgcolor"].Value == "#CCCCCC")
				{
					HtmlNodeCollection htmlTDNodes = node.SelectNodes("td");

					//Departments row has 2 table cells
					if(htmlTDNodes.Count == 2)
					{
						//Add previously collected course information
						if(department != null)
						{
							if(addNewCourse)
							{
								course.Sections.Add(section);
								courses.Add(course);
								readAdditionalInfo = false;
								addNewCourse = false;
							}

							department.Courses = courses;
							semester.Departments.Add(department);
						}

						department = new Department();
						course = null;
						section = null;
						courses = new List<Course>();
					}
					else
					{
						continue;
					}

					foreach(HtmlNode tdNode in htmlTDNodes)
					{
						if(tdNode.NodeType == HtmlNodeType.Element && tdNode.HasAttributes)
						{
							//Check if there is colspan skipping data blocks
							if(tdNode.Attributes["colspan"] != null && !string.IsNullOrEmpty(tdNode.Attributes["colspan"].Value))
							{
								int currentCell;
								int.TryParse(tdNode.Attributes["colspan"].Value, out currentCell);
								string text = tdNode.InnerText.CleanHTMLSpecialCharacters();

								if(currentCell == 2 && !string.IsNullOrEmpty(text))
								{
									department.Id = text;
								}
								else if(currentCell == 10 && !string.IsNullOrEmpty(text))
								{
									department.Name = text.RemoveValues("Courses", department.Id);
								}
							}
						}
					}

				}
				else if(node.NodeType == HtmlNodeType.Element && node.HasAttributes && node.Attributes["bgcolor"].Value == "#DFE4FF")
				{
					//Add previously collected course information
					if(course != null)
					{
						courses.Add(course);
						readAdditionalInfo = false;
						addNewCourse = false;
					}

					string[] value = node.InnerText.Trim().Replace("&nbsp;", string.Empty).Split(new string[] { "\r\n\t\t\t" }, StringSplitOptions.RemoveEmptyEntries);

					if(value != null && value.Length == 3)
					{
						course = new Course();

						//Department
						course.Department = value[0].Trim().Replace(@"&nbsp", string.Empty);

						//courseID and Name
						string[] courseIDandName = value[1].Split(new string[] { "&#150;" }, StringSplitOptions.None);
						course.CourseID = courseIDandName[0].Trim();
						course.CourseName = courseIDandName[1].Trim();

						//Number of credits
						course.Credits = value[2].RemoveValues("(", "Credits", ")");

						addNewCourse = true;
						readAdditionalInfo = true;
						section = new Section();
					}
				}
				else if(readAdditionalInfo && node.NodeType == HtmlNodeType.Element && node.HasAttributes && (node.Attributes["bgcolor"].Value == "#E1E1CC" || node.Attributes["bgcolor"].Value == "#FFFFFF"))
				{
					//Read Additional information
					int currentCell = 1;
					int columnsSkipped = 0;
					section = new Section();

					HtmlNodeCollection htmlTDNodes = node.SelectNodes("td");

					foreach(HtmlNode tdNode in htmlTDNodes)
					{
						if(tdNode.NodeType == HtmlNodeType.Element && tdNode.HasAttributes)
						{
							//Check if there is colspan skipping data blocks
							if(tdNode.Attributes["colspan"] != null && !string.IsNullOrEmpty(tdNode.Attributes["colspan"].Value))
							{
								int.TryParse(tdNode.Attributes["colspan"].Value, out columnsSkipped);

								currentCell = currentCell + (columnsSkipped - 1);

								//We are continuing the previous section, get the last section
								if(columnsSkipped == 3)
								{
									section = course.Sections[course.Sections.Count - 1];
								}
							}
						}

						string innerText = HttpUtility.HtmlDecode(tdNode.InnerText).Trim();

						switch(currentCell)
						{
							case 1:
								section.SectionID = string.IsNullOrEmpty(section.SectionID) ? innerText : section.SectionID;
								break;
							case 2:
								break;
							case 3:
								section.GradeMethod = string.IsNullOrEmpty(section.GradeMethod) ? innerText : section.GradeMethod;
								break;
							case 4:
								section.Days.Add(string.Join(" ", (IEnumerable<char>)innerText.Replace(" ", string.Empty)));
								break;
							case 5:
								section.Time.Add(innerText);
								break;
							case 6:
								section.Dates.Add(innerText);
								break;
							case 7:
								section.Room.Add(innerText);
								break;
							case 8:
								section.Instructor.Add(innerText);
								break;
							case 9:
								int size = 0;
								int.TryParse(innerText, out size);
								section.Size = size;
								break;
							case 10:
								int enrolled = 0;
								int.TryParse(innerText, out enrolled);
								course.Enrolled = enrolled;
								break;
							case 11:
								course.Status = innerText;
								break;
							case 12: //END of ROW
								if(columnsSkipped == 0)
								{
									course.Sections.Add(section);
								}
								break;
							default:
								break;
						}

						currentCell++;
					}
				}
			}

			//Add last collected information
			if(department != null)
			{
				if(addNewCourse)
				{
					course.Sections.Add(section);
					courses.Add(course);
					readAdditionalInfo = false;
					addNewCourse = false;
				}

				department.Courses = courses;
				semester.Departments.Add(department);
			}

			return semester;
		}

		/// <summary>
		/// Retrieves the course data.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
		private string RetrieveRawCourseData(string semesterID)
		{
			RestClientSettings settings = new RestClientSettings();

			settings.URL = "http://www3.mnsu.edu/courses/selectform.asp";
			settings.Method = Method.POST;

			//add valid header for kicks although they are not looking for it
			settings.Parameters.Add(new Parameter() { Name = "Accept", Value = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", Type = ParameterType.HttpHeader });
			settings.Parameters.Add(new Parameter() { Name = "Accept-Encoding", Value = "gzip, deflate", Type = ParameterType.HttpHeader });
			settings.Parameters.Add(new Parameter() { Name = "Accept-Language", Value = "en-US,en;q=0.5", Type = ParameterType.HttpHeader });
			settings.Parameters.Add(new Parameter() { Name = "Host", Value = "www3.mnsu.edu", Type = ParameterType.HttpHeader });
			settings.Parameters.Add(new Parameter() { Name = "Referer", Value = "http://www3.mnsu.edu/courses/", Type = ParameterType.HttpHeader });
			settings.Parameters.Add(new Parameter() { Name = "User-Agent", Value = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0", Type = ParameterType.HttpHeader });

			// adds to POST or URL query string based on Method
			settings.Parameters.Add(new Parameter() { Name = "All", Value = "All Sections", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "campus", Value = "1,2,3,4,5,6,7,9,A,B,C,I,L,M,N,P,Q,R,S,T,W,U,V,X,Z", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "college", Value = "", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "courseid", Value = "", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "courselevel", Value = "", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "coursenum", Value = "", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "days", Value = "ALL", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "endTime", Value = "2359", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "semester", Value = semesterID, Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "startTime", Value = "0600", Type = ParameterType.GetOrPost });
			settings.Parameters.Add(new Parameter() { Name = "subject", Value = "", Type = ParameterType.GetOrPost });

			return CommonFunctions.MakeRestSharpRequest(settings);
		}
		#endregion		
	}
}