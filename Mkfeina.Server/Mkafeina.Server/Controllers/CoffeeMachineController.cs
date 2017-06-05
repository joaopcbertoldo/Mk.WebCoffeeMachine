using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using System.Web.Http;

namespace Mkafeina.Server.Controllers
{
	public class CoffeeMachineController : ApiController
	{
		[HttpPost]
		[Route("api/coffeemachine/registration")]
		public RegistrationResponse Registration([FromBody] RegistrationRequest request)
		{
			return CMProxyHub.Sgt.HandleRegistrationRequest(request);
		}

		[HttpPost]
		[Route("api/coffeemachine/report")]
		public ReportResponse Report([FromBody] ReportRequest request)
		{
			return CMProxyHub.Sgt.HandleReportRequest(request);
		}

		[HttpPost]
		[Route("api/coffeemachine/order")]
		public OrderResponse Order([FromBody] OrderRequest request)
		{
			return CMProxyHub.Sgt.HandleOrderRequest(request);
		}
	}
}