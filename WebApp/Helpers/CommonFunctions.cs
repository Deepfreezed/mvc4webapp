using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Net.Cache;
using System.Web.Mvc;

namespace WebApp.Helpers
{
	public class CommonFunctions
	{
		/// <summary>
		/// Makes the HTTP web request.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public static string MakeHttpWebRequest(string url, string start, string end)
		{
			string results = string.Empty;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "GET";
			httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

			//httpWebRequest.ContentType = "application/x-www-form-urlencoded";

			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			StreamReader streamReader = new StreamReader(responseStream);
			string response = streamReader.ReadToEnd();

			int startIndex = response.IndexOf(start);
			int endIndex = response.IndexOf(end);

			if(startIndex > 0 && endIndex > 0 && (endIndex > startIndex))
			{
				results = response.Substring(startIndex, endIndex - startIndex);
			}

			return results;
		}

		public static string MakeHttpWebRequest(string url, string start, string end, string proxyIP, int proxyPort)
		{
			string results = string.Empty;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Proxy = new WebProxy(proxyIP, proxyPort);
			httpWebRequest.Method = "GET";
			httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

			//httpWebRequest.ContentType = "application/x-www-form-urlencoded";

			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			StreamReader streamReader = new StreamReader(responseStream);
			string response = streamReader.ReadToEnd();

			int startIndex = response.IndexOf(start);
			int endIndex = response.IndexOf(end);

			if(startIndex > 0 && endIndex > 0 && (endIndex > startIndex))
			{
				results = response.Substring(startIndex, endIndex - startIndex);
			}

			return results;
		}

		/// <summary>
		/// Creates the sessions.
		/// </summary>
		/// <param name="item">The item.</param>
		public static void StoreCookie(string key, object value)
		{
			DateTime SessionExpiration = DateTime.UtcNow.AddHours(24);

			//Save user ID cookie
			HttpCookie authCookie = new HttpCookie(key, value.ToString())
			{
				Expires = SessionExpiration
			};

			HttpContext.Current.Response.Cookies.Add(authCookie);
		}

		/// <summary>
		/// Creates the sessions.
		/// </summary>
		/// <param name="item">The item.</param>
		public static string ReadCookie(string key)
		{
			HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

			if(cookie != null)
			{
				return cookie.Value;
			}
			else
			{
				return string.Empty;
			}
		}

		public static readonly IDictionary<string, string> StateDictionary = new Dictionary<string, string> {
			{"ALABAMA", "AL"},
			{"ALASKA", "AK"},
			{"ARIZONA ", "AZ"},
			{"ARKANSAS", "AR"},
			{"CALIFORNIA ", "CA"},
			{"COLORADO ", "CO"},
			{"CONNECTICUT", "CT"},
			{"DELAWARE", "DE"},
			{"DISTRICT OF COLUMBIA", "DC"},
			{"FLORIDA", "FL"},
			{"GEORGIA", "GA"},
			{"GUAM ", "GU"},
			{"HAWAII", "HI"},
			{"IDAHO", "ID"},
			{"ILLINOIS", "IL"},
			{"INDIANA", "IN"},
			{"IOWA", "IA"},
			{"KANSAS", "KS"},
			{"KENTUCKY", "KY"},
			{"LOUISIANA", "LA"},
			{"MAINE", "ME"},
			{"MARYLAND", "MD"},
			{"MASSACHUSETTS", "MA"},
			{"MICHIGAN", "MI"},
			{"MINNESOTA", "MN"},
			{"MISSISSIPPI", "MS"},
			{"MISSOURI", "MO"},
			{"MONTANA", "MT"},
			{"NEBRASKA", "NE"},
			{"NEVADA", "NV"},
			{"NEW HAMPSHIRE", "NH"},
			{"NEW JERSEY", "NJ"},
			{"NEW MEXICO", "NM"},
			{"NEW YORK", "NY"},
			{"NORTH CAROLINA", "NC"},
			{"NORTH DAKOTA", "ND"},
			{"OHIO", "OH"},
			{"OKLAHOMA", "OK"},
			{"OREGON", "OR"},
			{"PALAU", "PW"},
			{"PENNSYLVANIA", "PA"},
			{"PUERTO RICO", "PR"},
			{"RHODE ISLAND", "RI"},
			{"SOUTH CAROLINA", "SC"},
			{"SOUTH DAKOTA", "SD"},
			{"TENNESSEE", "TN"},
			{"TEXAS", "TX"},
			{"UTAH", "UT"},
			{"VERMONT", "VT"},
			{"VIRGIN ISLANDS", "VI"},
			{"VIRGINIA", "VA"},
			{"WASHINGTON", "WA"},
			{"WEST VIRGINIA", "WV"},
			{"WISCONSIN", "WI"},
			{"WYOMING", "WY"}
		};

		public static SelectList StateSelectList
		{
			get { return new SelectList(StateDictionary, "Value", "Key"); }
		}
	}
}