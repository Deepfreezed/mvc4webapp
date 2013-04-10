using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace WebApp.Helpers
{
	public static class ExtensionMethods
	{
		/// <summary>
		/// Strips the HTML.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		public static string StripHtml(this string input)
		{
			// Will this simple expression replace all tags???
			var tagsExpression = new Regex(@"</?.+?>");
			return tagsExpression.Replace(input, " ");
		}

		/// <summary>
		/// Cleans the HTML special characters.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		public static string CleanHTMLSpecialCharacters(this string input)
		{			
			if(!string.IsNullOrEmpty(input))
			{
				input = HttpUtility.HtmlDecode(input).Trim();
			}

			return input;
			//return Regex.Replace(input, "[^a-zA-Z0-9% ._]", string.Empty)
		}

		/// <summary>
		/// Replaces the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="args">The vales to replace.</param>
		/// <returns></returns>
		public static string RemoveValues(this string input, params object[] args)
		{
			if(!string.IsNullOrEmpty(input))
			{
				foreach(string value in args)
				{
					input = input.Replace(value, string.Empty);
				}
			}

			return input.Trim();
		}

		/// <summary>
		/// Toes the lookup.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <returns></returns>
		public static ILookup<string, string> ToLookup(this NameValueCollection collection)
		{
			if(collection == null)
			{
				throw new ArgumentNullException("collection");
			}

			var pairs =
				from key in collection.Cast<String>()
				from value in collection.GetValues(key)
				select new { key, value };

			return pairs.ToLookup(pair => pair.key, pair => pair.value);
		}
	}
}