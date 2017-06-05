using Mkafeina.Server.Domain;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Mkafeina.Server.Controllers
{
	public class TestController : ApiController
	{
		[Route("test")]
		public HttpResponseMessage Get()
		{
			Dashboard.Sgt.LogAsync($"Test request received at {DateTime.UtcNow}");
			return Request.CreateResponse(System.Net.HttpStatusCode.OK, new { message = "Oi, quer café?" });
		}
	}
}