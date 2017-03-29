using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mkfeina.Server.Controllers
{
    public class TestController : ApiController
    {
        [Route("test")]
        public HttpResponseMessage Get() {
            Console.Clear();
            Console.WriteLine(Request.ToString());
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, new { message = "got your request" });
        }
    }
}
