using RazorEngine;
using RazorEngine.Templating;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using RazorEngine.Configuration;
using System.Reflection;

namespace Mkfeina.Server.Controllers
{
	public class HomeController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage Index()
		{
			var templateConfig = new TemplateServiceConfiguration();
			var templateManager = new DelegateTemplateManager(name =>
			{
				string resourcePath = $"Mkfeina.Server.Views.{name}";
				var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
				using (StreamReader reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			});
			var template = templateManager.Resolve(templateManager.GetKey("Index.cshtml", ResolveType.Global, null));

			var model = new { Title = "MKafeína", Email = "exemplo@gmail.com" };

			string result = Engine.Razor.RunCompile(template, "key", null, model);

			return new HttpResponseMessage
			{
				Content = new StringContent(result, System.Text.Encoding.UTF8, "text/html")
			};
		}
	}
}