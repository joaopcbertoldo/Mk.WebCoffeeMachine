using Mkfeina.Domain.ServerArduinoComm;
using Mkfeina.Server.Domain;
using System.Web.Http;

namespace Mkfeina.Server.Controllers
{
#warning add logs partout

	public class CoffeeMachineController : ApiController
	{
		private bool HashVerified(string guid, int hash)
		{
			return true;
#warning implementar hash e add config de enable/disable do hash + add hash no cache
		}

		[HttpPost]
		[Route("api/coffeemachine/registration")]
#warning evoluir para ihttpactionresult
		public RegistrationResponse Registration([FromBody] RegistrationRequest request)
		{
			switch ((RegistrationMessageEnum)request.RegistrationMessage)
			{
				case RegistrationMessageEnum.Undefined:
					return new RegistrationResponse() { ResponseCode = (int)ResponseCodeEnum.InvalidRequest };

				case RegistrationMessageEnum.AttemptRegistration:
					return CoffeeMachineProxy.HandleRegistrationAttempt(request);

				case RegistrationMessageEnum.RegistrationAcceptance:
					return CoffeeMachineProxy.HandleRegistrationAcceptance(request);

				case RegistrationMessageEnum.Offsets:
#warning fazer Offsets
					return new RegistrationResponse() { ResponseCode = (int)ResponseCodeEnum.InvalidRequest };

				case RegistrationMessageEnum.Unregister:
#warning fazer Unregister
					return new RegistrationResponse() { ResponseCode = (int)ResponseCodeEnum.InvalidRequest };

				default:
					return new RegistrationResponse() { ResponseCode = (int)ResponseCodeEnum.InvalidRequest };
			}
		}

		[HttpPost]
		[Route("api/coffeemachine/report")]
#warning evoluir para ihttpactionresult
		public ReportResponse Report([FromBody] ReportRequest request)
		{
			var mac = request.Mac;
			if (CoffeeMachineProxy.RegistrationStatus(mac) != RegistrationStatusEnum.Registered)
				return new ReportResponse()
				{
					ResponseCode = (int)ResponseCodeEnum.InvalidRequest
				};
			else
				return CoffeeMachineProxy.GetProxy(mac).HandleReportRequest(request);
		}

		[HttpPost]
		[Route("api/coffeemachine/order")]
#warning evoluir para ihttpactionresult
		public OrderResponse Order([FromBody] OrderRequest request)
		{
			var mac = request.Mac;
			if (CoffeeMachineProxy.RegistrationStatus(mac) != RegistrationStatusEnum.Registered)
				return new OrderResponse()
				{
					ResponseCode = (int)ResponseCodeEnum.InvalidRequest
				};
			else
				return CoffeeMachineProxy.GetProxy(mac).HandleOrderRequest(request);
		}
	}
}