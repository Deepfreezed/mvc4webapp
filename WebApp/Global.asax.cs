using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApp
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		protected void Session_Start(object sender, EventArgs e) 
		{
			//HttpCookie cookie = new HttpCookie("ASP.NET_SessionId", Session.SessionID.ToString()); 
			//cookie.Expires = DateTime.Now.AddMinutes(20); 
			//cookie.Domain = "*.janithtest.com"; 
			//cookie.HttpOnly = true; 
			//Response.SetCookie(cookie); 
		}
	}
}