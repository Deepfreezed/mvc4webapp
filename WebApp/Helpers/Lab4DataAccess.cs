using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using WebApp.Models;
using Raven.Json.Linq;

namespace WebApp.Helpers
{
	public class Lab4DataAccess
	{
		private const string SessionKey = "Lab4ModelUserID";
		private const int SessionTimeOutValue = 10;
		private IDocumentSession m_RavenSession;

		public Lab4DataAccess(IDocumentSession RavenSession)
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
		/// Gets the session time out.
		/// </summary>
		public string SessionTimeOut
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Retrieve from session.
		/// </summary>
		/// <returns></returns>
		public PrintingCalculator RetrieveFromSession()
		{			
			//Read the session cookie
			PrintingCalculator item;

			if(!string.IsNullOrEmpty(UserID))
			{
				item = m_RavenSession.Load<PrintingCalculator>(UserID);
			}
			else
			{
				DateTime SessionExpiration = DateTime.UtcNow.AddMinutes(SessionTimeOutValue);

				//initialize a link list with max 20 operations
				item = new PrintingCalculator();

				m_RavenSession.Store(item);
				m_RavenSession.Advanced.GetMetadataFor(item)["Raven-Expiration-Date"] = new RavenJValue(SessionExpiration);
				m_RavenSession.SaveChanges();

				//Save user ID cookie
				HttpCookie authCookie = new HttpCookie(SessionKey, m_RavenSession.Advanced.GetDocumentId(item))
				{
					Expires = SessionExpiration
				};

				HttpContext.Current.Response.Cookies.Add(authCookie);
			}

			return item;
		}

		/// <summary>
		/// Clears from session.
		/// </summary>
		public void ClearFromSession()
		{
			PrintingCalculator item = RetrieveFromSession();
			m_RavenSession.Delete<PrintingCalculator>(item);
		}

		/// <summary>
		/// Gets the running calculations.
		/// </summary>
		/// <returns></returns>
		public string GetRunningCalculations()
		{
			PrintingCalculator item = RetrieveFromSession();
			string msg = string.Empty;

			foreach(string str in item.RunningCalculations)
			{
				msg = msg + str;
			}

			return msg;
		}
	}
}