using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;

namespace WebApp.Models
{
	public class RestClientSettings
	{
		public RestClientSettings()
		{
			Parameters = new List<Parameter>();
		}

		public string URL { get; set; }
		public Method Method { get; set; }
		public List<Parameter> Parameters { get; set; }
		public string ProxyIP { get; set; }
		public int ProxyPort { get; set; }
	}
}