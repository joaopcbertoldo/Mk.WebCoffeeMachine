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
            Console.WriteLine("Inside Test Api Controller...");
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine($"REQUEST HEADERS");
            foreach (var h in Request.Headers) {
                Console.WriteLine($"{h.Key} : {string.Join(", ", h.Value)}");
            }
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine();
            return Ok(new { message = $"OK, i got your request on <<{Request.RequestUri.AbsolutePath}>>" });
        }
    }
}