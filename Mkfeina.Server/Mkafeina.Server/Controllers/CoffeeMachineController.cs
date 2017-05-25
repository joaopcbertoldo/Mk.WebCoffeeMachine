using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain;
using Mkfeina.Domain.ServerArduinoComm;
using Mkfeina.Server.Domain;
using System.Web.Http;

namespace Mkafeina.Server.Controllers
{
#warning add logs partout

	public class CoffeeMachineController : ApiController
	{
		private ArduinoResponseFactory _ardResponseFac = new ArduinoResponseFactory();

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
			var mac = request.Mac;
			if (!CMProxyHub.Sgt.IsRegistered(mac))
				return _ardResponseFac.ReportInvalidRequest(); 
			else
				return CMProxyHub.Sgt.GetProxy(mac).HandleReportRequest(request);
		}

		[HttpPost]
		[Route("api/coffeemachine/order")]
		public OrderResponse Order([FromBody] OrderRequest request)
		{
			var mac = request.Mac;
			if (CMProxy.RegistrationStatus(mac) != RegistrationStatusEnum.Registered)
				return new OrderResponse()
				{
					ResponseCode = (int)ResponseCodeEnum.InvalidRequest
				};
			else
				return CMProxy.GetProxy(mac).HandleOrderRequest(request);
		}
	}
}