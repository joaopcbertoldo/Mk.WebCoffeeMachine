using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using System.Web.Http;

namespace Mkafeina.Server.Controllers
{
	public class CoffeeMachineController : ApiController
	{
		public static ArduinoResponseFactory _ardResponseFac = new ArduinoResponseFactory();

		private TResponse MacNotRegistered<TResponse>() where TResponse : ArduinoResponse
			=> _ardResponseFac.InvalidRequest<TResponse>(ErrorEnum.MacNotRegistered, CommandEnum.Register);

		[Route("api/coffeemachine/registration")]
		public RegistrationResponse Post([FromBody] RegistrationRequest request)
		{
			switch (request.Msg)
			{
				case MessageEnum.Registration:
					return CMProxyHub.Sgt.HandleRegistration(request);

				case MessageEnum.Offsets:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleOffsets(request) ?? MacNotRegistered<RegistrationResponse>();

				case MessageEnum.Unregistration:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleUnregistration(request) ?? MacNotRegistered<RegistrationResponse>();

				default:
					return _ardResponseFac.InvalidRequest<RegistrationResponse>(ErrorEnum.UnknownMessage);
			}
		}

		[Route("api/coffeemachine/report")]
		public ReportResponse Post([FromBody] ReportRequest request)
		{
			switch (request.Msg)
			{
				case MessageEnum.Signals:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleSignals(request) ?? MacNotRegistered<ReportResponse>();

				case MessageEnum.Disabling:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleDisabling(request) ?? MacNotRegistered<ReportResponse>();

				case MessageEnum.Reenable:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleReenable(request) ?? MacNotRegistered<ReportResponse>();

				default:
					return _ardResponseFac.InvalidRequest<ReportResponse>(ErrorEnum.UnknownMessage);
			}
		}

		[Route("api/coffeemachine/order")]
		public OrderResponse Post([FromBody] OrderRequest request)
		{
			switch (request.Msg)
			{
				case MessageEnum.GiveMeAnOrder:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleGiveMeAnOrder(request) ?? MacNotRegistered<OrderResponse>();

				case MessageEnum.Ready:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleReady(request) ?? MacNotRegistered<OrderResponse>();

				case MessageEnum.CancelOrders:
					return CMProxyHub.Sgt.GetProxy(request.Mac)?.HandleCancelOrder(request) ?? MacNotRegistered<OrderResponse>();

				default:
					return _ardResponseFac.InvalidRequest<OrderResponse>(ErrorEnum.UnknownMessage);
			}
		}
	}
}