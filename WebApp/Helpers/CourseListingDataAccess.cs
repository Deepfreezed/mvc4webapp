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
			currentSemesterID = "20151Summer 2014";

			//Current year past semesters
			currentSemesterIDs.Add("20145Spring 2014");

			//Previous year archived Semesters
			previousSemestersIDs.Add("20141Summer 2013");
			previousSemestersIDs.Add("20135Spring 2013");
			previousSemestersIDs.Add("20133Fall 2012");
			previousSemestersIDs.Add("20131Summer 2012");
			previousSemestersIDs.Add("20125Spring 2012");
			previousSemestersIDs.Add("20123Fall 2011");
			previousSemestersIDs.Add("20121Summer 2011");
			previousSemestersIDs.Add("20115Spring 2011");
			previousSemestersIDs.Add("20113Fall 2010");
			previousSemestersIDs.Add("20111Summer 2010");
			previousSemestersIDs.Add("20105Spring 2010");
			previousSemestersIDs.Add("20103Fall 2009");
		}

		/// <summary>
		/// Gets all semesters.
		/// </summary>
		/// <returns></returns>
		public List<Semester> GetAllSemesters()
		{
			return m_RavenSession.Query<Semester>().ToList();
		}

		/// <summary>
		/// Gets all departments by semester ID.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
		public List<Department> GetAllDepartmentsBySemesterID(string semesterID)
		{
			return m_RavenSession.Query<Department>("DepartmentIndex").Where(d => d.SemesterID.Equals(semesterID, StringComparison.InvariantCultureIgnoreCase)).ToList();
		}

		/// <summary>
		/// Gets all departments for current semester.
		/// </summary>
		/// <returns></returns>
		public List<Department> GetAllDepartmentsForCurrentSemester()
		{
			string semester = currentSemesterID.Remove(0, 5).Replace(" ", string.Empty);

			return m_RavenSession.Query<Department>("DepartmentIndex").Where(d => d.SemesterID.Equals(semester, StringComparison.InvariantCultureIgnoreCase)).ToList();
		}

		/// <summary>
		/// Gets all courses by semester ID.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
		public List<Course> GetAllCoursesBySemesterID(string semesterID)
		{
			return m_RavenSession.Query<Course>("CourseIndex").Where(c => c.SemesterID.Equals(semesterID, StringComparison.InvariantCultureIgnoreCase)).ToList();
		}

		/// <summary>
		/// Gets the semester by ID.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
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
		/// Gets the courses by semester ID and department ID.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <param name="departmentID">The department ID.</param>
		/// <returns></returns>
		public List<Course> GetCoursesBySemesterIDandDepartmentID(string semesterID, string departmentID)
		{
			List<Course> courses = new List<Course>();

			if(!string.IsNullOrEmpty(semesterID) && !string.IsNullOrEmpty(departmentID))
			{
				Department department = m_RavenSession.Query<Department>("DepartmentIndex")
					.FirstOrDefault(d => d.SemesterID.Equals(semesterID, StringComparison.InvariantCultureIgnoreCase) && d.DepartmentID.Equals(departmentID, StringComparison.InvariantCultureIgnoreCase));

				if(department != null && department.CourseIds.Count > 0)
				{
					courses = m_RavenSession.Load<Course>(department.CourseIds.ToArray()).ToList();
				}
			}

			return courses;
		}

		/// <summary>
		/// Gets the courses.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <param name="departmentID">The department ID.</param>
		/// <param name="courseID">The course ID.</param>
		/// <returns></returns>
		public List<Course> GetCourses(string semesterID, string departmentID, string courseID)
		{
			List<Course> courses = new List<Course>();

			if(!string.IsNullOrEmpty(semesterID) && !string.IsNullOrEmpty(departmentID) && !string.IsNullOrEmpty(courseID))
			{
				courses = m_RavenSession.Query<Course>("CourseIndex")
					.Where(c => 
						c.SemesterID.Equals(semesterID, StringComparison.InvariantCultureIgnoreCase) && 
						c.DepartmentID.Equals(departmentID, StringComparison.InvariantCultureIgnoreCase) &&
						c.CourseNumber.Equals(courseID, StringComparison.InvariantCultureIgnoreCase)).ToList();
			}

			return courses;
		}

		/// <summary>
		/// Gets the course rating by ID.
		/// </summary>
		/// <param name="courseID">The course ID.</param>
		/// <returns></returns>
		public Rating GetCourseRatingByID(string courseID)
		{
			int rating = 0;
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
			//Delete data from database
			DeleteAllCourseData();

			if(!string.IsNullOrEmpty(currentSemesterID))
			{
				RetrieveAndSaveSemesterData(currentSemesterID);				
			}

			//Current Year semesters
			foreach(string semesterID in currentSemesterIDs)
			{
				RetrieveAndSaveSemesterData(semesterID);
			}

			//Past Semesters
			foreach(string semesterID in previousSemestersIDs)
			{
				RetrieveAndSaveSemesterData(semesterID, true);
			}

			PopulateHistoricalData();
		}

		/// <summary>
		/// Loads the semester to database.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		public void LoadSemesterToDatabase(string semesterID)
		{
			if(!string.IsNullOrEmpty(semesterID))
			{
				string semester = semesterID.Remove(0, 5).Replace(" ", string.Empty);
				DeleteSemesterByID(semester);

				if(currentSemesterID.Equals(semesterID, StringComparison.InvariantCultureIgnoreCase) || currentSemesterIDs.Any(s => s.Equals(semesterID, StringComparison.InvariantCultureIgnoreCase)))
				{
					RetrieveAndSaveSemesterData(semesterID);
				}
				else
				{
					RetrieveAndSaveSemesterData(semesterID, true);
				}
			}
		}

		/// <summary>
		/// Populates the historical data.
		/// </summary>
		/// <param name="currentSemester">The current semester.</param>
		/// <param name="previousSemesters">The previous semesters.</param>
		/// <returns></returns>
		public void PopulateHistoricalData()
		{
			string semesterID = currentSemesterID.Remove(0, 5).Replace(" ", string.Empty);

			//Get previous semester courses
			List<string> previousSemesterIDs = new List<string>();
			previousSemesterIDs.AddRange(currentSemesterIDs.Select(s => s = s.Remove(0, 5).Replace(" ", string.Empty)));
			previousSemesterIDs.AddRange(previousSemestersIDs.Select(s => s = s.Remove(0, 5).Replace(" ", string.Empty)));

			if(previousSemesterIDs.Count > 0)
			{
				//Get Current semester courses
				Semester currentSemester = m_RavenSession.Include<Semester>(s => s.DepartmentIds).Load<Semester>(semesterID);
				List<Department> currentDepartments = m_RavenSession.Include<Department>(d => d.CourseIds).Load<Department>(currentSemester.DepartmentIds.ToArray()).ToList();
				List<Course> currentCourses = m_RavenSession.Load<Course>(currentDepartments.SelectMany(d => d.CourseIds).ToArray()).ToList();

				List<Semester> previousSemesters = m_RavenSession.Include<Semester>(s => s.DepartmentIds).Load<Semester>(previousSemesterIDs).ToList();
				previousSemesters.RemoveAll(s => s == null);

				if(previousSemesters != null && previousSemesters.Count > 0)
				{
					List<Department> previousSemesterDepartments = m_RavenSession.Include<Department>(d => d.CourseIds).Load<Department>(previousSemesters.SelectMany(s => s.DepartmentIds).ToArray()).ToList();
					List<Course> previousSemesterCourses = m_RavenSession.Load<Course>(previousSemesterDepartments.SelectMany(d => d.CourseIds).ToArray()).ToList();

					foreach(Course course in currentCourses)
					{
						List<Section> sections = previousSemesterCourses.Where(c => c.CourseID == course.CourseID).SelectMany(s => s.Sections).ToList();

						int size = sections.Sum(s => s.Size);
						int enrolled = sections.Sum(s => s.Enrolled);

						course.AddEnrolmentStatics(enrolled, size);
					}

					//m_RavenSession.Store(currentCourses);
					m_RavenSession.SaveChanges();
				}				
			}
		}

		/// <summary>
		/// Deletes all course data.
		/// </summary>
		public void DeleteAllCourseData()
		{
			m_RavenSession.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("AllDocumentsById", new IndexQuery());			
			m_RavenSession.SaveChanges();
		}

		/// <summary>
		/// Deletes the semester by ID.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		public void DeleteSemesterByID(string semesterID)
		{
			List<Department> departments = GetAllDepartmentsBySemesterID(semesterID);
			List<Course> courses = GetAllCoursesBySemesterID(semesterID);
			Semester semester = GetSemesterByID(semesterID);

			//Delete courses
			foreach(Course course in courses)
			{
				m_RavenSession.Delete<Course>(course);
			}

			//Delete departments
			foreach(Department department in departments)
			{
				m_RavenSession.Delete<Department>(department);
			}

			//Delete Semester
			if(semester != null)
			{
				m_RavenSession.Delete<Semester>(semester);
			}			

			//Commit
			m_RavenSession.SaveChanges();			
		}

		#region Private Methods

		/// <summary>
		/// Retrieves the and save semester data.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
		private Semester RetrieveAndSaveSemesterData(string semesterID, bool isArchiveData = false)
		{
			Semester semester = new Semester();
			semester.Id = semesterID.Remove(0, 5).Replace(" ", string.Empty);
			semester.Name = semesterID.Remove(0, 5);

			Dictionary<string, string> departmentsValues = RetrieveDepartmentList();

			foreach(KeyValuePair<string, string> departmentValue in departmentsValues)
			{
				Department department = new Department();
				department.DepartmentID = departmentValue.Key;
				department.Name = departmentValue.Value;
				department.SemesterID = semester.Id;

				string content = string.Empty;

				if(isArchiveData)
				{
					content = RetrieveRawCourseData(semesterID, department.DepartmentID, "http://www3.mnsu.edu/courses/selectformArchive.asp");
				}
				else
				{
					content = RetrieveRawCourseData(semesterID, department.DepartmentID);
				}
					

				ParseRawCourseData(content, department);

				//Store Department to database
				if(department.CourseIds.Count > 0)
				{
					m_RavenSession.Store(department);
					m_RavenSession.Advanced.AddCascadeDeleteReference(department, department.CourseIds.ToArray());
					semester.DepartmentIds.Add(department.Id);
				}				
			}

			//Store Semester to database			
			m_RavenSession.Store(semester);
			m_RavenSession.Advanced.AddCascadeDeleteReference(semester, semester.DepartmentIds.ToArray());

			//Save changes to database
			m_RavenSession.SaveChanges();

			return semester;
		}

		/// <summary>
		/// Parses the raw course data.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
		private Department ParseRawCourseData(string content, Department department)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(content);
			
			HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//table[2]/tr");			

			//Department department = null;
			Course course = null;
			Section section = null;
			bool readAdditionalInfo = false;
			bool addNewCourse = false;

			if(htmlNodes == null || htmlNodes.Count == 0)
			{
				return department;
			}

			foreach(HtmlNode node in htmlNodes)
			{				
				if(node.NodeType == HtmlNodeType.Element && node.HasAttributes && node.Attributes["bgcolor"].Value == "#DFE4FF")
				{
					//Add previously collected course information
					if(course != null)
					{
						//Store Course to database
						m_RavenSession.Store(course);
						department.CourseIds.Add(course.Id);

						readAdditionalInfo = false;
						addNewCourse = false;
						course = null;
					}

					string[] value = node.InnerText.Trim().Replace("&nbsp;", string.Empty).Split(new string[] { "\r\n\t\t\t" }, StringSplitOptions.RemoveEmptyEntries);

					if(value != null && value.Length == 3)
					{
						course = new Course();
						course.SemesterID = department.SemesterID;

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
									section = null;
								}
								else if(columnsSkipped == 12)
								{
									course.Sections[course.Sections.Count - 1].AdditionalNotes.Add(innerText);
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
					if(section != null)
					{
						course.Sections.Add(section);
						section = null;
					}					

					//Store Course to database
					m_RavenSession.Store(course);
					department.CourseIds.Add(course.Id);
					
					readAdditionalInfo = false;
					addNewCourse = false;
					course = null;
				}

				//Store Department to database
				//m_RavenSession.Store(department);
				//m_RavenSession.Advanced.AddCascadeDeleteReference(department, department.CourseIds.ToArray());

				//semester.DepartmentIds.Add(department.Id);
			}

			//Store Semester to database			
			//m_RavenSession.Store(semester);
			//m_RavenSession.Advanced.AddCascadeDeleteReference(semester, semester.DepartmentIds.ToArray());

			return department;
		}

		/// <summary>
		/// Retrieves the raw course data.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <param name="departmentID">The department ID.</param>
		/// <returns></returns>
		private string RetrieveRawCourseData(string semesterID, string departmentID)
		{
			return RetrieveRawCourseData(semesterID, departmentID, "http://www3.mnsu.edu/courses/selectform.asp");
		}

		/// <summary>
		/// Retrieves the raw course data.
		/// </summary>
		/// <param name="semesterID">The semester ID.</param>
		/// <returns></returns>
		private string RetrieveRawCourseData(string semesterID, string departmentID, string courseDataURL)
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
			settings.Parameters.Add(new Parameter() { Name = "subject", Value = departmentID, Type = ParameterType.GetOrPost });

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
				{ "IT201", 2},
				{ "IT202", 2},
				{ "IT219", 2},
				{ "IT310", 2},
				{ "IT311", 1},
				{ "IT320", 1},
				{ "IT321", 2},
				{ "IT412", 2},
				{ "IT414", 2},
				{ "IT430", 2},
				{ "IT432", 2},
				{ "IT440", 2},
				{ "IT442", 2},
				{ "IT444", 2},
				{ "IT450", 2},
				{ "IT460", 2},
				{ "IT462", 2},
				{ "IT480", 2},
				{ "IT482", 2},
				{ "IT486", 2},
				{ "IT488", 2},
				{ "IT512", 2},
				{ "IT514", 2},
				{ "IT530", 2},
				{ "IT532", 2},
				{ "IT544", 2},
				{ "IT550", 2},
				{ "IT564", 2},
				{ "IT580", 2},
				{ "IT582", 2},
				{ "IT583", 1},
				{ "IT584", 2},
				{ "IT600", 1},
				{ "IT601", 1},
				{ "IT602", 1},
				{ "IT630", 2},
				{ "IT640", 2},
				{ "IT641", 1},
				{ "IT662", 1},
				{ "IT680", 1},
				{ "IT690", 2}
			};

			return ratings;
		}

		/// <summary>
		/// Retrieves the department list.
		/// </summary>
		/// <returns></returns>
		private Dictionary<string, string> RetrieveDepartmentList()
		{
			Dictionary<string, string> departmentList = new Dictionary<string, string>()
			{
				{"ACCT","Accounting"},
				{"AET","Automotive Engineering Technology"},
				{"AIS","American Indian Studies"},
				{"ANTH","Anthropology"},
				{"ART","Art"},
				{"AST","Astronomy"},
				{"AVIA","Aviation Management"},
				{"BED","Business and Technology Education"},
				{"BIOL","Biology"},
				{"BLAW","Business Law"},
				{"CDIS","Communication Disorders"},
				{"CHEM","Chemistry"},
				{"CIVE","Civil Engineering"},
				{"CM","Construction Management"},
				{"CMST","Communication Studies"},
				{"CORR","Corrections"},
				{"CS","Computer Science"},
				{"CSP","Counseling and Student Personnel"},
				{"DANC","Dance"},
				{"DHYG","Dental Hygiene"},
				{"ECON","Economics"},
				{"ED","Education"},
				{"EDAD","Educational Administration"},
				{"EDLD","Educational Leadership"},
				{"EE","Electrical Engineering"},
				{"EEC","Educational Studies: Elementary and Early Childhood"},
				{"EET","Electronic Engineering Technology"},
				{"ENG","English"},
				{"ENVR","Environmental Sciences"},
				{"ESL","English as a Second Language"},
				{"ETHN","Ethnic Studies"},
				{"EXED","Experiential Education"},
				{"FCS","Family Consumer Science"},
				{"FINA","Finance"},
				{"FREN","French"},
				{"FYEX","First Year Experience"},
				{"GEOG","Geography"},
				{"GEOL","Geology"},
				{"GER","German"},
				{"GERO","Gerontology"},
				{"GWS","Gender and Women's Studies"},
				{"HIST","History"},
				{"HLTH","Health Science"},
				{"HONR","Honors Program"},
				{"HP","Human Performance"},
				{"HUM","Humanities"},
				{"IBUS","International Business"},
				{"ISYS","Information Systems"},
				{"IT","Information Technology"},
				{"KSP","Educational Studies: K-12 and Secondary Programs"},
				{"LATN","Latin"},
				{"LAWE","Law Enforcement"},
				{"MASS","Mass Communications"},
				{"MATH","Mathematics"},
				{"MBA","Master of Business Administration"},
				{"ME","Mechanical Engineering"},
				{"MEDT","Medical Technology"},
				{"MET","Manufacturing Engineering Technology"},
				{"MGMT","Management"},
				{"MODL","Modern Languages"},
				{"MRKT","Marketing"},
				{"MSL","Military Science"},
				{"MUS","Music"},
				{"NPL","Nonprofit Leadership"},
				{"NURS","Nursing"},
				{"OPEN","Open Studies"},
				{"PHIL","Philosophy"},
				{"PHYS","Physics"},
				{"POL","Political Science"},
				{"PSYC","Psychology"},
				{"REHB","Rehabilitation Counseling"},
				{"RPLS","Recreation  Parks and Liesure Services"},
				{"RUSS","Russian"},
				{"SCAN","Scandinavian Studies"},
				{"SOC","Sociology"},
				{"SOST","Social Studies"},
				{"SOWK","Social Work"},
				{"SPAN","Spanish"},
				{"SPED","Special Education"},
				{"STAT","Statistics"},
				{"THEA","Theatre"},
				{"URBS","Urban and Regional Studies"}
			};

			return departmentList;
		}
		#endregion		
	}
}