using System;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Mkafeina.Server.Controllers
{
	public class TestController : ApiController
	{
		private static bool _firstTestReceived = true;

		[Route("test")]
		public HttpResponseMessage Get()
		{
			//Console.Clear();
			//Console.WriteLine(Request.ToString());
			//if (_firstTestReceived)
			//{
			//	_firstTestReceived = false;
			//	Thread.Sleep(10000);
			//}
			return Request.CreateResponse(System.Net.HttpStatusCode.OK, new { message = "Oi, quer café?" });
		}
	}
}