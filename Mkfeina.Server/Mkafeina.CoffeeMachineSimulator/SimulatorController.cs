using System;
using System.Text;
using System.Web.Http;
using Mkfeina.Domain;

namespace Mkfeina.CoffeeMachineSimulator
{
    public class SimulatorController : ApiController
    {
        [Route("")]
        public IHttpActionResult Get()
        {
            FakeCoffeMachine.Singleton.LastReceivedRequest = DateTime.Now;
            Dashboard.LogAsync("GET request received.");
            return Ok(new { message = "one must complete this" });
        }

        [Route("")]
        public IHttpActionResult Post()
        {
            FakeCoffeMachine.Singleton.LastReceivedRequest = DateTime.Now;
            Dashboard.LogAsync("POST request received.");

            var buffer = Request.Content.ReadAsByteArrayAsync().Result;
            var content = Encoding.Default.GetString(buffer);
            var subStr = content.Substring(content.IndexOf("\"o\"") + 3);
            var operation = (char)subStr.Substring(subStr.IndexOf('\"') + 1, 1).ToCharArray().GetValue(0);

            switch (operation) {
                case 's':
                    return Ok(new { message = "one must complete this" });

                default:
                    return BadRequest();
            }
        }
    }
}