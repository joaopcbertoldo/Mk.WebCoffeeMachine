using Mkafeina.Server.Domain.CoffeeMachineProxy;
using Mkafeina.Server.Domain.CustomerApi;
using System.Web.Http;

namespace Mkafeina.Server.Controllers
{
	public class CustomerController : ApiController
	{
		private CustomerResponseFactory _custResponsefac = new CustomerResponseFactory();

		[HttpPost]
		[Route("api/customer/order")]
		public CustomerOrderResponse Order([FromBody] CustomerOrderRequest request)
		{
			var cmName = request.MachineUniqueName;
			var mac = CMProxyHub.Sgt.GetMac(cmName);
			if (mac == null)
				return _custResponsefac.InexistentCoffeeMachine();
			var response = Waitress.GetWaitress(mac)?.HandleCustomerOrder(request);
			return response;
		}
	}
}