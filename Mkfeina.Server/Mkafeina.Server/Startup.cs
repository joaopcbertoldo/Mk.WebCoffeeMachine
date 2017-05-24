using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.IO;
using System.Reflection;
using System.Web.Http;

namespace Mkfeina.Server
{
	public class Startup
	{
		// This code configures Web API. The Startup class is specified as a type
		// parameter in the WebApp.Start method.
		public void Configuration(IAppBuilder appBuilder)
		{
			// Configure Web API for self-host.
			HttpConfiguration config = new HttpConfiguration();

			config.Routes.MapHttpRoute("Default", "{controller}/{action}",
					new { controller = "Home", action = "Index" }
				);
			config.MapHttpAttributeRoutes();

			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

			var templateConfig = new TemplateServiceConfiguration();
			templateConfig.TemplateManager = new DelegateTemplateManager(name =>
			{
				string resourcePath = $"Mkfeina.Server.Views.{name}";
				var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
				using (StreamReader reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			});

			Engine.Razor = RazorEngineService.Create(templateConfig);

			appBuilder.UseWebApi(config);
		}
	}
}