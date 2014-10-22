using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Dynamic;
using System.Globalization;

namespace WebApp
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Resume", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "CatchAll",
				url: "{controller}/{action}/{*url}",
				defaults: new { controller = "Home", action = "Resume" }
			);
		}
	}

	public class ParsePathAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// Called by the ASP.NET MVC framework before the action method executes.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if(filterContext.RouteData.Values["url"] != null)
			{
				dynamic actionModel = new ExpandoObject() as IDictionary<string, Object>;
				string[] split = filterContext.RouteData.Values["url"].ToString().Split('/');

				for(int i = 0; i < split.Count(); i = i + 2)
				{
					if(((i + 1) < split.Count()) && !string.IsNullOrEmpty(split[i]) && !string.IsNullOrEmpty(split[i + 1]))
					{
						bool tryBool;
						float tryFloat;
						int tryInt;

						if(Boolean.TryParse(split[i + 1], out tryBool))
						{
							((IDictionary<string, Object>)actionModel).Add(split[i], tryBool);
							filterContext.ActionParameters[split[i]] = tryBool;							
						}
						else if(Int32.TryParse(split[i + 1], out tryInt))
						{
							((IDictionary<string, Object>)actionModel).Add(split[i], tryInt);
							filterContext.ActionParameters[split[i]] = tryInt;	
						}
						else if(float.TryParse(split[i + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out tryFloat))
						{
							((IDictionary<string, Object>)actionModel).Add(split[i], tryFloat);
							filterContext.ActionParameters[split[i]] = tryFloat;	
						}
						else
						{
							((IDictionary<string, Object>)actionModel).Add(split[i], split[i + 1]);
							filterContext.ActionParameters[split[i]] = split[i + 1];
						}
					}
				}

				filterContext.ActionParameters["actionModel"] = actionModel;
			}

			base.OnActionExecuting(filterContext);
		}
	}
}