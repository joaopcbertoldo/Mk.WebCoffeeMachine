using System;
using System.Web.Http;
using WebCoffeeMachine.Domain;
using WebCoffeeMachine.Server.Domain;
using WebCoffeeMachine.Server.Domain.ActionResults;

namespace WebCoffeeMachine.Server.Controllers
{
    [RoutePrefix("api")]
    public class CoffeeMachineController : ApiController
    {
        [HttpPost]
        [Route("register")]
        public IHttpActionResult Register([FromBody] RegistrationForms forms)
        {
            if (!forms.IsValid())
                return new RegistrationActionResult(Request, RegistrationResultStatusEnum.InvalidForms);

            if (Cache.Singleton.CoffeeMachines.ContainsKey(forms.UniqueName))
                return new RegistrationActionResult(Request, RegistrationResultStatusEnum.UniqueNameAlreadyTaken);

            int communicationPin;
            Cache.Singleton.CoffeeMachines.Add(forms.UniqueName, new CoffeeMachineProxy(forms.Ip, forms.Port, out communicationPin));

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