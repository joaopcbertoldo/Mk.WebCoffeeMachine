using System;
using System.Web.Http;

namespace HttpListenerTest
{
    public class TestApiController : ApiController
    {
        // GET
        public object Get()
        {
            Console.WriteLine();
            Console.WriteLine("From controller... --------------------------------------------------");
            Console.WriteLine($"Request Headers : {Request.Headers}");
            Console.WriteLine($"Request Content : {Request.Content.ToString()}");
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine();
            return Ok(new { message = $"OK, i got your request on <<{Request.RequestUri.AbsolutePath}>>" });
        }
    }
}