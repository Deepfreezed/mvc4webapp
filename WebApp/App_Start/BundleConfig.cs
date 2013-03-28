using System.Web;
using System.Web.Optimization;

namespace WebApp
{
	public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/bootstrap.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
						"~/Scripts/jquery-ui-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.unobtrusive*",
						"~/Scripts/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/custom/lab4").Include(
						"~/Scripts/custom/lab4-functions.js"));

			bundles.Add(new ScriptBundle("~/bundles/custom/lab5").Include(
						"~/Scripts/custom/lab5-functions.js"));

			bundles.Add(new ScriptBundle("~/bundles/custom/assignment2").Include(
						"~/Scripts/jquery.blockUI.js",
						"~/Scripts/custom/assignment2-functions.js"));

			bundles.Add(new ScriptBundle("~/bundles/custom/assignment3").Include(
						"~/Scripts/jquery.dataTables.js",
						"~/Scripts/custom/assignment3-functions.js"));
			
			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

			bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
						"~/Content/themes/base/jquery.ui.core.css",
						"~/Content/themes/base/jquery.ui.resizable.css",
						"~/Content/themes/base/jquery.ui.selectable.css",
						"~/Content/themes/base/jquery.ui.accordion.css",
						"~/Content/themes/base/jquery.ui.autocomplete.css",
						"~/Content/themes/base/jquery.ui.button.css",
						"~/Content/themes/base/jquery.ui.dialog.css",
						"~/Content/themes/base/jquery.ui.slider.css",
						"~/Content/themes/base/jquery.ui.tabs.css",
						"~/Content/themes/base/jquery.ui.datepicker.css",
						"~/Content/themes/base/jquery.ui.progressbar.css",
						"~/Content/themes/base/jquery.ui.theme.css"));

			bundles.Add(new StyleBundle("~/academichonesty/css").Include("~/Content/academichonesty/academichonesty.css"));

			bundles.Add(new StyleBundle("~/studentconductprocess/css").Include("~/Content/studentconductprocess/studentconductprocess.css"));

			bundles.Add(new StyleBundle("~/assignment2/css").Include("~/Content/assignment2/assignment2.css"));

			bundles.Add(new StyleBundle("~/lab5/css").Include("~/Content/lab5/lab5.css"));

			bundles.Add(new StyleBundle("~/assignment3/css").Include(
						"~/Content/assignment3/assignment3.css",
						"~/Content/assignment3/dataTables.airports.css"));

			bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
						"~/Content/bootstrap.css",
						"~/Content/bootstrap-responsive.css",						
						"~/Content/font-awesome.css"));
		}
	}
}