using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Models;
using RestSharp;
using HtmlAgilityPack;
using WebApp.Models.CourseListing;
using Raven.Client;
using System.Collections.Specialized;
using Raven.Abstractions.Data;

namespace WebApp.Helpers
{
	public class CourseListingDataAccess
	{
		private IDocumentSession m_RavenSession;
		private string currentSemesterID = string.Empty;
		private List<string> currentSemesterIDs = new List<string>();
		private List<string> previousSemestersIDs = new List<string>();
		
		public CourseListingDataAccess(IDocumentSession RavenSession)
		{
			m_RavenSession = RavenSession;

			//Current Semester
			currentSemesterID = "20143Fall 2013";

			//Current year past semesters
			currentSemesterIDs.Add("20141Summer 2013");
			currentSemesterIDs.Add("20135Spring 2013");

			//Previous year archived Semesters
			previousSemestersIDs.Add("20133Fall 2012");
			//previousSemestersIDs.Add("20131Summer 2012");
			//previousSemestersIDs.Add("20125Spring 2012");
			//previousSemestersIDs.Add("20123Fall 2011");
			//previousSemestersIDs.Add("20121Summer 2011");
			//previousSemestersIDs.Add("20115Spring 2011");
			//previousSemestersIDs.Add("20113Fall 2010");
			//previousSemestersIDs.Add("20111Summer 2010");
			//previousSemestersIDs.Add("20105Spring 2010");
			//previousSemestersIDs.Add("20103Fall 2009");
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

			//if(!string.IsNullOrEmpty(semesterID) && !string.IsNullOrEmpty(departmentID))
			//{
			//    Semester semester = m_RavenSession.Load<Semester>(semesterID);
			//    Department department = semester.Departments.FirstOrDefault(d => d.Id.Equals(departmentID, StringComparison.InvariantCultureIgnoreCase));

			//    if(department != null && department.Courses.Count > 0)
			//    {
			//        courses = department.Courses;
			//    }					
			//}

			return courses;
		}

		/// <summary>
		/// Gets the course rating by ID.
		/// </summary>
		/// <param name="courseID">The course ID.</param>
		/// <returns></returns>
		public Rating GetCourseRatingByID(string courseID)
		{
			int rating = 1;
			RetrieveCourseRatings().TryGetValue(courseID, out rating);

			return (Rating)rating;
		}

		/// <summary>
		/// Gets the course prerequisites by ID.
		/// </summary>
		/// <param name="courseID">The course ID.</param>
		/// <returns></returns>
		public List<string> GetCoursePrerequisitesByID(string courseID)
		{
			List<KeyValuePair<string, string>> prerequisites = RetrievePrerequisites();
			List<string> returnValues = new List<string>();

			foreach(KeyValuePair<string, string> keyvalue in prerequisites)
			{
				if(keyvalue.Key.Equals(courseID, StringComparison.InvariantCultureIgnoreCase))
				{
					returnValues.Add(keyvalue.Value);
				}
			}

			return returnValues;
		}

		/// <summary>
		/// Loads all course data to database.
		/// </summary>
		public void LoadAllCourseDataToDatabase()
		{
			Semester currentSemester = new Semester();
			List<Semester> previousSemesters = new List<Semester>();

			//Delete data from database
			DeleteAllCourseData();

			if(!string.IsNullOrEmpty(currentSemesterID))
			{
				string data = RetrieveRawCourseData(currentSemesterID);
				currentSemester = ParseAndSaveRawCourseData(data, currentSemesterID);				
			}

			//Current Year semesters
			foreach(string semesterID in currentSemesterIDs)
			{
				string data = RetrieveRawCourseData(semesterID);
				Semester semester = ParseAndSaveRawCourseData(data, semesterID);

				previousSemesters.Add(semester);
			}

			//Past Semesters
			foreach(string semesterID in previousSemestersIDs)
			{
				string data = RetrieveRawCourseData(semesterID, "http://www3.mnsu.edu/courses/selectformArchive.asp");
				Semester semester = ParseAndSaveRawCourseData(data, semesterID);

				previousSemesters.Add(semester);
			}

			//PopulateHistoricalData(currentSemester, previousSemesters);
		}

		/// <summary>
		/// Deletes all course data.
		/// </summary>
		public void DeleteAllCourseData()
		{
			m_RavenSession.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("AllDocumentsById", new IndexQuery());			
			m_RavenSession.SaveChanges();
		}

		#region Private Methods

		/// <summary>
		/// Parses the raw course data.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
		private Semester ParseAndSaveRawCourseData(string content, string semesterID)
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

			if(htmlNodes == null || htmlNodes.Count == 0)
			{
				return semester;
			}

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
								
								//Save Course to database
								m_RavenSession.Store(course);								
								department.CourseIds.Add(course.Id);
								
								readAdditionalInfo = false;
								addNewCourse = false;
							}

							//Store Department to database
							m_RavenSession.Store(department);
							m_RavenSession.Advanced.AddCascadeDeleteReference(department, department.CourseIds.ToArray());
							semester.DepartmentIds.Add(department.Id);
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
									department.DepartmentID = text;
								}
								else if(currentCell == 10 && !string.IsNullOrEmpty(text))
								{
									department.Name = text.RemoveValues("Courses", department.DepartmentID);
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
						//Store Course to database
						m_RavenSession.Store(course);
						department.CourseIds.Add(course.Id);

						readAdditionalInfo = false;
						addNewCourse = false;
					}

					string[] value = node.InnerText.Trim().Replace("&nbsp;", string.Empty).Split(new string[] { "\r\n\t\t\t" }, StringSplitOptions.RemoveEmptyEntries);

					if(value != null && value.Length == 3)
					{
						course = new Course();

						//Department
						course.DepartmentID = value[0].Trim().Replace(@"&nbsp", string.Empty);

						//courseID and Name
						string[] courseIDandName = value[1].Split(new string[] { "&#150;" }, StringSplitOptions.None);
						course.CourseNumber = courseIDandName[0].Trim();
						course.CourseName = courseIDandName[1].Trim();

						//Number of credits
						course.Credits = value[2].RemoveValues("(", "Credits", ")");

						//Course Pre-requisite
						course.Prerequisites = GetCoursePrerequisitesByID(course.CourseID);

						//Course rating
						course.CourseRating = GetCourseRatingByID(course.CourseID);

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
								section.Enrolled = enrolled;
								break;
							case 11:
								section.Status = innerText;
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

					//Store Course to database
					m_RavenSession.Store(course);
					department.CourseIds.Add(course.Id);
					
					readAdditionalInfo = false;
					addNewCourse = false;
				}

				//Store Department to database
				m_RavenSession.Store(department);
				m_RavenSession.Advanced.AddCascadeDeleteReference(department, department.CourseIds.ToArray());

				semester.DepartmentIds.Add(department.Id);
			}

			//Store Semester to database
			semester.Id = semesterID;
			semester.Name = semesterID.Remove(0, 5);
			m_RavenSession.Store(semester);
			m_RavenSession.Advanced.AddCascadeDeleteReference(semester, semester.DepartmentIds.ToArray());

			//Save changes to database
			m_RavenSession.SaveChanges();

			return semester;
		}

		/// <summary>
		/// Retrieves the course data.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
		private string RetrieveRawCourseData(string semesterID)
		{
			return RetrieveRawCourseData(semesterID, "http://www3.mnsu.edu/courses/selectform.asp");
		}

		/// <summary>
		/// Retrieves the raw course data.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
		private string RetrieveRawCourseData(string semesterID, string courseDataURL)
		{
			RestClientSettings settings = new RestClientSettings();

			settings.URL = courseDataURL;
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

		/// <summary>
		/// Retrieves the prerequisites.
		/// </summary>
		/// <returns></returns>
		private List<KeyValuePair<string, string>> RetrievePrerequisites()
		{
			List<KeyValuePair<string, string>> prerequisites = new List<KeyValuePair<string, string>>() 
			{ 
				new KeyValuePair<string, string>("IT214", "IT210"),
				new KeyValuePair<string, string>("IT310", "IT214"),
				new KeyValuePair<string, string>("IT311", "IT214"),
				new KeyValuePair<string, string>("IT320", "IT214"),
				new KeyValuePair<string, string>("IT340", "IT210"),
				new KeyValuePair<string, string>("IT350", "IT210"),
				new KeyValuePair<string, string>("IT360", "IT210"),
				new KeyValuePair<string, string>("IT380", "IT214"),
				new KeyValuePair<string, string>("IT412", "IT214"),
				new KeyValuePair<string, string>("IT414", "IT340"),
				new KeyValuePair<string, string>("IT414", "IT310"),
				new KeyValuePair<string, string>("IT430", "IT214"),
				new KeyValuePair<string, string>("IT432", "IT320"),
				new KeyValuePair<string, string>("IT440", "IT214"),
				new KeyValuePair<string, string>("IT440", "IT340"),
				new KeyValuePair<string, string>("IT442", "IT350"),
				new KeyValuePair<string, string>("IT442", "IT440"),
				new KeyValuePair<string, string>("IT444", "IT440"),
				new KeyValuePair<string, string>("IT450", "IT350"),
				new KeyValuePair<string, string>("IT460", "IT214"),
				new KeyValuePair<string, string>("IT460", "IT360"),
				new KeyValuePair<string, string>("IT462", "IT350"),
				new KeyValuePair<string, string>("IT462", "IT460"),
				new KeyValuePair<string, string>("IT464", "IT460"),
				new KeyValuePair<string, string>("IT480", "IT380"),
				new KeyValuePair<string, string>("IT482", "IT380"),
				new KeyValuePair<string, string>("IT483", "IT340"),
				new KeyValuePair<string, string>("IT483", "IT380"),
				new KeyValuePair<string, string>("IT484", "IT380"),
				new KeyValuePair<string, string>("IT486", "IT380"),
				new KeyValuePair<string, string>("IT488", "IT340"),
				new KeyValuePair<string, string>("IT488", "IT380"),
				new KeyValuePair<string, string>("IT630", "IT530"),
				new KeyValuePair<string, string>("IT640", "IT540"),
				new KeyValuePair<string, string>("IT641", "IT540"),
				new KeyValuePair<string, string>("IT662", "IT562"),
				new KeyValuePair<string, string>("IT662", "IT564"),
				new KeyValuePair<string, string>("IT680", "IT580")
 
			};

			return prerequisites;
		}

		/// <summary>
		/// Retrieves the course ratings.
		/// </summary>
		/// <returns></returns>
		private Dictionary<string, int> RetrieveCourseRatings()
		{
			Dictionary<string, int> ratings = new Dictionary<string, int>()
			{				
				{ "IT201", 3},
				{ "IT202", 3},
				{ "IT219", 3},
				{ "IT310", 3},
				{ "IT311", 2},
				{ "IT320", 2},
				{ "IT321", 3},
				{ "IT412", 3},
				{ "IT414", 3},
				{ "IT430", 3},
				{ "IT432", 3},
				{ "IT440", 3},
				{ "IT442", 3},
				{ "IT444", 3},
				{ "IT450", 3},
				{ "IT460", 3},
				{ "IT462", 3},
				{ "IT480", 2},
				{ "IT482", 2},
				{ "IT486", 3},
				{ "IT488", 3},
				{ "IT512", 3},
				{ "IT514", 3},
				{ "IT530", 3},
				{ "IT532", 3},
				{ "IT544", 3},
				{ "IT550", 3},
				{ "IT564", 3},
				{ "IT580", 2},
				{ "IT582", 3},
				{ "IT583", 2},
				{ "IT584", 3},
				{ "IT600", 2},
				{ "IT601", 2},
				{ "IT602", 2},
				{ "IT630", 3},
				{ "IT640", 3},
				{ "IT641", 2},
				{ "IT662", 2},
				{ "IT680", 2},
				{ "IT690", 3}
			};

			return ratings;
		}

		/// <summary>
		/// Populates the historical data.
		/// </summary>
		/// <param name="currentSemester">The current semester.</param>
		/// <param name="previousSemesters">The previous semesters.</param>
		/// <returns></returns>
		private Semester PopulateHistoricalData(Semester currentSemester, List<Semester> previousSemesters)
		{
			//foreach(Department department in currentSemester.Departments)
			//{
			//    List<Course> previousSemesterCourses = previousSemesters.SelectMany(s => s.Departments).SelectMany(d => d.Courses).ToList();

			//    foreach(Course course in department.Courses)
			//    {
			//        List<Section> previousSemesterSections = previousSemesterCourses.Where(c => c.CourseID == course.CourseID).SelectMany(s => s.Sections).ToList();

			//        int size = previousSemesterSections.Sum(s => s.Size);
			//        int enrolled = previousSemesterSections.Sum(s => s.Enrolled);

			//        course.AddEnrolmentStatics(enrolled, size);
			//    }
			//}

			//m_RavenSession.Store(currentSemester);
			//m_RavenSession.SaveChanges();

			return currentSemester;
		}
		#endregion		
	}
}