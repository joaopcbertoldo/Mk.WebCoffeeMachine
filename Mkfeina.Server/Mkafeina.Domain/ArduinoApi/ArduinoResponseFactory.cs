using Mkafeina.Domain.ServerArduinoComm;
using System;

namespace Mkafeina.Domain.ArduinoApi
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

		public OrderResponse GiveMeAnOrderOK(string orderRef, string recipeStr)
			=> new OrderResponse()
			{
				Command = CommandEnum.Process,
				ResponseCode = ResponseCodeEnum.OK,
				Error = ErrorEnum.Void,
				OrderReference = orderRef,
				RecipeStr = recipeStr
			};

		public OrderResponse GiveMeAnOrderAgain(string orderRef, string recipeStr)
			=> new OrderResponse()
			{
				Command = CommandEnum.Process,
				ResponseCode = ResponseCodeEnum.InvalidRequest,
				Error = ErrorEnum.OrderAlreadyTaken,
				OrderReference = orderRef,
				RecipeStr = recipeStr
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