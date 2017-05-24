using Mkfeina.Server.Domain;
using RazorEngine;
using System.Net.Http;
using System.Web.Http;

namespace Mkfeina.Server.Controllers
{
	public class CustomerController : ApiController
	{
		[HttpPost]
		[Route("api/customer/order")]
#warning evoluir para ihttpactionresult
		public CustomerOrderResponse Order([FromBody] CustomerOrderRequest request)
		{
			if (CoffeeMachineProxy.MachineIsRegistered(request.MachineUniqueName))
				return CoffeeMachineProxy.GetProxyByUniqueName(request.MachineUniqueName).HandleClientOrderRequest(request);
			else
				return new CustomerOrderResponse()
				{
					Message = "Error: inexistent coffee machine."
				};
		}

	}
}