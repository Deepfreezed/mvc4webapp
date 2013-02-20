using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace WebApp.Models
{
	public class Helpers
	{
	}

	public static class SessionExtender
	{
		/// <summary>
		/// Adds the with timeout.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <param name="expireAfter">The expire after.</param>
		public static void AddWithTimeout(this HttpSessionState session, string name, object value,	TimeSpan expireAfter)
		{
			session[name] = value;
			session[name + "ExpDate"] = DateTime.Now.Add(expireAfter);
		}

		/// <summary>
		/// Gets the with timeout.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static object GetWithTimeout(this HttpSessionState session, string name)
		{
			object value = session[name];
			if(value == null)
			{
				return null;
			}				

			DateTime? expDate = session[name + "ExpDate"] as DateTime?;
			if(expDate == null)
			{
				return null;
			}				

			if(expDate < DateTime.Now)
			{
				session.Remove(name);
				session.Remove(name + "ExpDate");
				return null;
			}

			return value;
		}

		/// <summary>
		/// Gets the session time out.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static string GetSessionTimeOut(this HttpSessionState session, string name)
		{
			DateTime? expDate = session[name + "ExpDate"] as DateTime?;			
			string expirationDateTime = string.Empty;

			if(expDate != null)
			{
				expirationDateTime = ((DateTime)expDate).ToString(@"yyyy/MM/dd HH:mm:ss");
			}

			return expirationDateTime;
		}
	}
}