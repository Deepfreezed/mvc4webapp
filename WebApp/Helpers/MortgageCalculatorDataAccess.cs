using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using WebApp.Models.Mortgage;

namespace WebApp.Helpers
{
	public class MortgageCalculatorDataAccess
	{
		private const string SessionKey = "MortgageCalculatorUserID";
		private const int SessionTimeOutValue = 6000;
		private IDocumentSession m_RavenSession;

		public MortgageCalculatorDataAccess(IDocumentSession RavenSession)
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
		public MortgageUser RetrieveFromSession()
		{
			//Read the session cookie
			MortgageUser item = null;

			if(!string.IsNullOrEmpty(UserID))
			{
				item = m_RavenSession.Load<MortgageUser>(UserID);				
			}

			return item;
		}

		public void SignOutSession()
		{
			//Save user ID cookie
			HttpCookie authCookie = new HttpCookie(SessionKey)
			{
				Expires = DateTime.Now.AddDays(-1d),
				Value = string.Empty
			};

			HttpContext.Current.Response.Cookies.Add(authCookie);
		}

		/// <summary>
		/// Gets the user.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <returns></returns>
		public MortgageUser GetUser(string userName)
		{
			return m_RavenSession.Query<MortgageUser>("MortgageUserIndex").FirstOrDefault(m => m.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
		}

		/// <summary>
		/// Saves the user.
		/// </summary>
		/// <param name="user">The user.</param>
		public void SaveUser(MortgageUser user)
		{
			m_RavenSession.Store(user);
			m_RavenSession.SaveChanges();

			CreateSessions(user);
		}

		/// <summary>
		/// Creates the sessions.
		/// </summary>
		/// <param name="item">The item.</param>
		public void CreateSessions(MortgageUser item)
		{
			DateTime SessionExpiration = DateTime.UtcNow.AddMonths(SessionTimeOutValue);

			//Save user ID cookie
			HttpCookie authCookie = new HttpCookie(SessionKey, m_RavenSession.Advanced.GetDocumentId(item))
			{
				Expires = SessionExpiration
			};

			HttpContext.Current.Response.Cookies.Add(authCookie);
		}

		/// <summary>
		/// Deletes the current user.
		/// </summary>
		public void DeleteCurrentUser()
		{
			MortgageUser user = RetrieveFromSession();
			m_RavenSession.Delete<MortgageUser>(user);
			SignOutSession();
			m_RavenSession.SaveChanges();
		}

		/// <summary>
		/// Saves the changes.
		/// </summary>
		public void SaveChanges()
		{
			m_RavenSession.SaveChanges();
		}
	}
}