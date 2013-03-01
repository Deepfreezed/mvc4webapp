using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using WebApp.Models.Assignment2;

namespace WebApp.Helpers
{
	public class Assignment2DataAccess
	{
		private const string SessionKey = "Assignment2UserID";
		private const int SessionTimeOutValue = 12;
		private IDocumentSession m_RavenSession;

		public Assignment2DataAccess(IDocumentSession RavenSession)
		{
			m_RavenSession = RavenSession;
		}
		
		/// <summary>
		/// Gets the user ID.
		/// </summary>
		public string UserID
		{
			get
			{
				HttpCookie cookie = HttpContext.Current.Request.Cookies[SessionKey];

				if(cookie != null)
				{
					return cookie.Value;
				}
				else
				{
					return string.Empty;
				}

			}
		}

		/// <summary>
		/// Retrieves from session.
		/// </summary>
		/// <returns></returns>
		public MathPractice RetrieveFromSession()
		{
			//Read the session cookie
			MathPractice item;

			if(!string.IsNullOrEmpty(UserID))
			{
				item = m_RavenSession.Load<MathPractice>(UserID);

				if(item == null || item.ID < 0)
				{					
					item = new MathPractice();
					item.CreateQuestions();

					CreateSessions(item);
				}
			}
			else
			{
				//initialize a link list with max 20 operations
				item = new MathPractice();
				item.CreateQuestions();

				CreateSessions(item);
			}

			return item;
		}

		/// <summary>
		/// Creates the new math practice.
		/// </summary>
		/// <returns></returns>
		public MathPractice CreateNewSession()
		{
			MathPractice item = new MathPractice();
			item.CreateQuestions();

			CreateSessions(item);

			return item;
		}

		/// <summary>
		/// Creates the sessions.
		/// </summary>
		/// <param name="item">The item.</param>
		private void CreateSessions(MathPractice item)
		{
			DateTime SessionExpiration = DateTime.UtcNow.AddMonths(SessionTimeOutValue);

			m_RavenSession.Store(item);
			m_RavenSession.SaveChanges();

			//Save user ID cookie
			HttpCookie authCookie = new HttpCookie(SessionKey, m_RavenSession.Advanced.GetDocumentId(item))
			{
				Expires = SessionExpiration
			};

			HttpContext.Current.Response.Cookies.Add(authCookie);
		}
	}
}