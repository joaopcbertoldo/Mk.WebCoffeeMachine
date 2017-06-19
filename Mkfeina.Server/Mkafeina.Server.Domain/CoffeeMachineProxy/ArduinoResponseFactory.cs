using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.ServerArduinoComm;
using System;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public class ArduinoResponseFactory
	{
		public TResponse InvalidRequest<TResponse>(ErrorEnum error = ErrorEnum.Void, CommandEnum command = CommandEnum.Void)
			where TResponse : ArduinoResponse
		{
			var response = (TResponse)typeof(TResponse).GetConstructor(new Type[0]).Invoke(null);
			response.c = command;
			response.e = error;
			response.rc = ResponseCodeEnum.InvalidRequest;
			return response;
		}

		public RegistrationResponse RegistrationOK(CommandEnum command = CommandEnum.Void)
			=> new RegistrationResponse()
			{
				c = command,
				e = ErrorEnum.Void,
				rc = ResponseCodeEnum.OK
			};

		public ReportResponse ReportOK(CommandEnum command = CommandEnum.Void)
			=> new ReportResponse()
			{
				c = command,
				e = ErrorEnum.Void,
				rc = ResponseCodeEnum.OK
			};

		// OLD VERSION
		//public OrderResponse GiveMeAnOrderOK(string orderRef, string recipe)
		public OrderResponse GiveMeAnOrderOK(string orderRef, RecipeObj recipe)
			=> new OrderResponse()
			{
				c = CommandEnum.Process,
				rc = ResponseCodeEnum.OK,
				e = ErrorEnum.Void,
				oref = orderRef,
				rec = recipe
			};

		// OLD VERSION
		//public OrderResponse GiveMeAnOrderAgain(string orderRef, string recipe)
		public OrderResponse GiveMeAnOrderAgain(string orderRef, RecipeObj recipe)
			=> new OrderResponse()
			{
				c = CommandEnum.Process,
				rc = ResponseCodeEnum.InvalidRequest,
				e = ErrorEnum.OrderAlreadyTaken,
				oref = orderRef,
				rec = recipe
			};

		public OrderResponse ReadyOK()
			=> new OrderResponse()
			{
				c = CommandEnum.Void,
				rc = ResponseCodeEnum.OK,
				e = ErrorEnum.Void
			};

		public OrderResponse CancelOrderResponse(ErrorEnum error)
			=> new OrderResponse()
			{
				c = CommandEnum.Disable,
				e = error,
				rc = error == ErrorEnum.Void ? ResponseCodeEnum.OK : ResponseCodeEnum.InvalidRequest
			};
	}
}