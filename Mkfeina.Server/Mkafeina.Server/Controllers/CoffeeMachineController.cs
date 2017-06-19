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
			switch (request.msg)
			{
				case MessageEnum.Registration:
					return CMProxyHub.Sgt.HandleRegistration(request);

				case MessageEnum.Offsets:
					return CMProxyHub.Sgt.GetProxy(request.mac)?.HandleOffsets(request) ?? MacNotRegistered<RegistrationResponse>();

				case MessageEnum.Unregistration:
					return CMProxyHub.Sgt.GetProxy(request.mac)?.HandleUnregistration(request) ?? MacNotRegistered<RegistrationResponse>();

				default:
					return _ardResponseFac.InvalidRequest<RegistrationResponse>(ErrorEnum.UnknownMessage);
			}
		}

		[Route("api/coffeemachine/report")]
		public ReportResponse Post([FromBody] ReportRequest request)
		{
			switch (request.msg)
			{
				case MessageEnum.Signals:
					return CMProxyHub.Sgt.GetProxy(request.mac)?.HandleSignals(request) ?? MacNotRegistered<ReportResponse>();

				case MessageEnum.Disabling:
					return CMProxyHub.Sgt.GetProxy(request.mac)?.HandleDisabling(request) ?? MacNotRegistered<ReportResponse>();

				case MessageEnum.Reenable:
					return CMProxyHub.Sgt.GetProxy(request.mac)?.HandleReenable(request) ?? MacNotRegistered<ReportResponse>();

				default:
					return _ardResponseFac.InvalidRequest<ReportResponse>(ErrorEnum.UnknownMessage);
			}
		}

		[Route("api/coffeemachine/order")]
		public OrderResponse Post([FromBody] OrderRequest request)
		{
			switch (request.msg)
			{
				case MessageEnum.GiveMeAnOrder:
					return CMProxyHub.Sgt.GetProxy(request.mac)?.HandleGiveMeAnOrder(request) ?? MacNotRegistered<OrderResponse>();

				case MessageEnum.Ready:
					return CMProxyHub.Sgt.GetProxy(request.mac)?.HandleReady(request) ?? MacNotRegistered<OrderResponse>();

				case MessageEnum.CancelOrders:
					return CMProxyHub.Sgt.GetProxy(request.mac)?.HandleCancelOrder(request) ?? MacNotRegistered<OrderResponse>();

				default:
					return _ardResponseFac.InvalidRequest<OrderResponse>(ErrorEnum.UnknownMessage);
			}
		}
	}
}