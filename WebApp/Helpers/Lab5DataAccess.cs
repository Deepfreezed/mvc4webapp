using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Raven.Client;
using WebApp.ViewModels;

namespace WebApp.Helpers
{
	public class Lab5DataAccess
	{
		private const string SessionKey = "Lab5UserID";
		private const int SessionTimeOutValue = 12;
		private IDocumentSession m_RavenSession;

		public Lab5DataAccess(IDocumentSession RavenSession)
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
		public Lab5ViewModel RetrieveFromSession()
		{
			//Read the session cookie
			Lab5ViewModel item;

			if(!string.IsNullOrEmpty(UserID))
			{
				item = m_RavenSession.Load<Lab5ViewModel>(UserID);

				if(item == null || item.ID < 0)
				{
					item = new Lab5ViewModel();

					CreateSessions(item);
				}
			}
			else
			{
				//initialize a link list with max 20 operations
				item = new Lab5ViewModel();

				CreateSessions(item);
			}

			return item;
		}

		/// <summary>
		/// Creates the sessions.
		/// </summary>
		/// <param name="item">The item.</param>
		private void CreateSessions(Lab5ViewModel item)
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