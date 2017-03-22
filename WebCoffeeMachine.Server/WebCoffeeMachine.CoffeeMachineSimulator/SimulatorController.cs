using System.Text;
using System.Web.Http;
using WebCoffeeMachine.Domain;

namespace WebCoffeeMachine.CoffeeMachineSimulator
{
    public class SimulatorController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok(new { panel = Simulator.Singleton.ToPanel() });
        }

        public IHttpActionResult Post()
        {
            var buffer = Request.Content.ReadAsByteArrayAsync().Result;
            var content = Encoding.Default.GetString(buffer);
            var subStr = content.Substring(content.IndexOf("\"o\"") + 3);
            var operation = (char)subStr.Substring(subStr.IndexOf('\"') + 1, 1).ToCharArray().GetValue(0);

            switch (operation) {
                case 's':
                    return Ok(Simulator.Singleton.ToPanel().PanelToString());

                default:
                    return BadRequest();
            }
        }
    }
}