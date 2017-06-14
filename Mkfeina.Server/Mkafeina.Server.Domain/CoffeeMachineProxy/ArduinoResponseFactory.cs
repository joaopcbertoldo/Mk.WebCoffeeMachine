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
			response.Command = command;
			response.Error = error;
			response.ResponseCode = ResponseCodeEnum.InvalidRequest;
			return response;
		}

		public RegistrationResponse RegistrationOK(CommandEnum command = CommandEnum.Void)
			=> new RegistrationResponse()
			{
				Command = command,
				Error = ErrorEnum.Void,
				ResponseCode = ResponseCodeEnum.OK
			};

		public ReportResponse ReportOK(CommandEnum command = CommandEnum.Void)
			=> new ReportResponse()
			{
				Command = command,
				Error = ErrorEnum.Void,
				ResponseCode = ResponseCodeEnum.OK
			};

		// OLD VERSION
		//public OrderResponse GiveMeAnOrderOK(string orderRef, string recipe)
		public OrderResponse GiveMeAnOrderOK(string orderRef, RecipeObj recipe)
			=> new OrderResponse()
			{
				Command = CommandEnum.Process,
				ResponseCode = ResponseCodeEnum.OK,
				Error = ErrorEnum.Void,
				OrderReference = orderRef,
				Recipe = recipe
			};

		// OLD VERSION
		//public OrderResponse GiveMeAnOrderAgain(string orderRef, string recipe)
		public OrderResponse GiveMeAnOrderAgain(string orderRef, RecipeObj recipe)
			=> new OrderResponse()
			{
				Command = CommandEnum.Process,
				ResponseCode = ResponseCodeEnum.InvalidRequest,
				Error = ErrorEnum.OrderAlreadyTaken,
				OrderReference = orderRef,
				Recipe = recipe
			};

		public OrderResponse ReadyOK()
			=> new OrderResponse()
			{
				Command = CommandEnum.Void,
				ResponseCode = ResponseCodeEnum.OK,
				Error = ErrorEnum.Void
			};

		public OrderResponse CancelOrderResponse(ErrorEnum error)
			=> new OrderResponse()
			{
				Command = CommandEnum.Disable,
				Error = error,
				ResponseCode = error == ErrorEnum.Void ? ResponseCodeEnum.OK : ResponseCodeEnum.InvalidRequest
			};
	}
}