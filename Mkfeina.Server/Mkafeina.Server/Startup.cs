using Mezm.Owin.Razor;
using Mezm.Owin.Razor.Routing;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Mkafeina.Server
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			// Configure Web API for self-host.
			HttpConfiguration config = new HttpConfiguration();

			config.MapHttpAttributeRoutes();

			//config.Routes.MapHttpRoute(
			//	name: "DefaultWeb",
			//	routeTemplate: "{controller}/{action}",
			//	defaults: new { controller = "Home", action = "Index" }
			//);

			var folders = new List<string>(){ "Resources", "Resources/Home" };
			foreach (var folder in folders)
			{
				var fileSystem = new PhysicalFileSystem("./" + folder);
				var options = new FileServerOptions
				{
					EnableDefaultFiles = true,
					FileSystem = fileSystem
				};
				app.UseFileServer(options);
			}

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

			app.UseWebApi(config);

			//app.UseErrorPage();
			//app.UseRazor(InitFileViewRoutes);
		}

		private static void InitFileViewRoutes(IRouteTable table)
		{
			table
				.AddFileRoute("/", "Views/Home/Index.cshtml")
				.AddFileRoute("/home/index", "Views/Home/Index.cshtml")
				.AddFileRoute("/home/about", "Views/Home/About.cshtml")
				.AddFileRoute("/home/contact", "Views/Home/Contact.cshtml");
		}
	}
}