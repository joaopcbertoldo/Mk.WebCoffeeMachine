using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using System.Web.Http;

namespace Mkafeina.Server.Controllers
{
	public class CoffeeMachineController : ApiController
	{
		public static ArduinoResponseFactory _ardResponseFac = new ArduinoResponseFactory();

		[HttpPost]
		[Route("api/coffeemachine/registration")]
		public RegistrationResponse Registration([FromBody] RegistrationRequest request)
		{
			switch (request.RegistrationMessage)
			{
				case RegistrationMessageEnum.AttemptRegistration:
					return CMProxyHub.Sgt.HandleRegistrationAttempt(request);

				case RegistrationMessageEnum.RegistrationAcceptance:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleRegistrationAcceptance(request) ?? _ardResponseFac.RegistrationInvalidRequest();

				case RegistrationMessageEnum.Offsets:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleOffsets(request) ?? _ardResponseFac.RegistrationInvalidRequest();

				case RegistrationMessageEnum.Unregister:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleUnregistration(request) ?? _ardResponseFac.RegistrationInvalidRequest();

				default:
					return _ardResponseFac.RegistrationInvalidRequest();
			}
		}

		[HttpPost]
		[Route("api/coffeemachine/report")]
		public ReportResponse Report([FromBody] ReportRequest request)
		{
			switch (request.ReportMessage)
			{
				case ReportMessageEnum.Levels:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleLevels(request) ?? _ardResponseFac.ReportInvalidRequest();

				case ReportMessageEnum.DisablingCoffeeMachine:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleDisabling(request) ?? _ardResponseFac.ReportInvalidRequest();

				default:
					return _ardResponseFac.ReportInvalidRequest();
			}
		}

		[HttpPost]
		[Route("api/coffeemachine/order")]
		public OrderResponse Order([FromBody] OrderRequest request)
		{
			switch (request.OrderMessage)
			{
				case OrderMessageEnum.GiveMeAnOrder:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleGiveMeAnOrder(request) ?? _ardResponseFac.OrderInvalidRequest();

				case OrderMessageEnum.ProcessingWillStart:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleProcessingWillStart(request) ?? _ardResponseFac.OrderInvalidRequest();

				case OrderMessageEnum.OrderReady:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleOrderReady(request) ?? _ardResponseFac.OrderInvalidRequest();

				case OrderMessageEnum.ProblemOcurredDuringProcessing:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleProblemOcurredDuringProcessing(request) ?? _ardResponseFac.OrderInvalidRequest();

				default:
					return _ardResponseFac.OrderInvalidRequest();
			}
		}
	}
}