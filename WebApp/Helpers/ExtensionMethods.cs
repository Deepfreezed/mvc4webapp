using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Raven.Client;
using Raven.Json.Linq;

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

		/// <summary>
		/// Adds the cascade delete reference.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="entity">The entity.</param>
		/// <param name="documentKeys">The document keys.</param>
		public static void AddCascadeDeleteReference(this IAdvancedDocumentSessionOperations session, object entity, params string[] documentKeys)
		{
			var metadata = session.GetMetadataFor(entity);
			
			if(metadata == null)
			{
				throw new InvalidOperationException("The entity must be tracked in the session before calling this method.");
			}

			if(documentKeys.Length == 0)
			{
				throw new ArgumentException("At least one document key must be specified.");
			}

			const string metadataKey = "Raven-Cascade-Delete-Documents";
			RavenJToken token;

			if(!metadata.TryGetValue(metadataKey, out token))
			{
				token = new RavenJArray();
			}				

			var list = (RavenJArray)token;
			foreach(var documentKey in documentKeys.Where(key => !list.Contains(key)))
			{
				list.Add(documentKey);
			}				

			metadata[metadataKey] = list;
		}
	}
}