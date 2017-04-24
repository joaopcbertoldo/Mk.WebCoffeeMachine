using Mkfeina.Domain.ServerArduinoComm;
using Mkfeina.Server.Attributes;
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
		[Route("registration")]
#warning evoluir para ihttpactionresult
		public RegistrationResponse Register([FromBody] RegistrationRequest request)
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
#warning fazer este
					return new RegistrationResponse() { ResponseCode = (int)ResponseCodeEnum.InvalidRequest };

				case RegistrationMessageEnum.Unregister:
#warning fazer este
					return new RegistrationResponse() { ResponseCode = (int)ResponseCodeEnum.InvalidRequest };

				default:
					return new RegistrationResponse() { ResponseCode = (int)ResponseCodeEnum.InvalidRequest };
			}
		}

		[HttpPost]
		[Route("report")]
#warning evoluir para ihttpactionresult
		public ReportResponse Report([FromBody] ReportRequest request)
		{
			return new ReportResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.OK,
				Command = (int)CommandEnum.DoNothing
			};
		}

		[HttpPost]
		[Route("order")]
#warning evoluir para ihttpactionresult
		public OrderResponse MakeCoffee([FromBody] OrderRequest request)
		{
			return new OrderResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.Cancel
			};
		}
	}
}