using System;
using System.Web.Http;
using Mkfeina.Domain.ServerArduinoComm;
using Mkfeina.Server.Domain;
using Mkfeina.Server.Domain.ActionResults;

namespace Mkfeina.Server.Controllers
{
    public class CoffeeMachineController : ApiController
    {
        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register([FromBody] RegistrationRequest registration)
        {
			//Console.Clear();
			Console.WriteLine(Request.ToString());
			Console.WriteLine(registration);

            if (Cache.Singleton.CoffeeMachines.ContainsKey(registration.un))
                return new RegistrationActionResult(Request, RegistrationResultStatusEnum.UniqueNameAlreadyTaken);

            int communicationPin;
            Cache.Singleton.CoffeeMachines.Add(registration.un, new CoffeeMachineProxy(registration.i, registration.p, out communicationPin));

            return new RegistrationActionResult(Request, RegistrationResultStatusEnum.Ok, communicationPin);
        }

        [HttpGet]
        [Route("{uniqueName}/status")]
        public IHttpActionResult Status([FromUri] string uniqueName)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("{uniqueName}/makeCoffee")]
        public IHttpActionResult MakeCoffee([FromUri] string uniqueName)
        {
            throw new NotImplementedException();
        }

    }
}