using System;
using Mkafeina.Domain.ServerArduinoComm;

namespace Mkafeina.Domain.ArduinoApi
{
	public class ArduinoResponseFactory
	{
		public RegistrationResponse RegistrationAttemptWithMacAlreadyExisting(bool alreadyAccepted)
			=> new RegistrationResponse()
			{
				ResponseCode = alreadyAccepted ? (int)RegistrationResponseCodeEnum.AlreadyRegistered :
												   (int)RegistrationResponseCodeEnum.RegisteredButNotAccepted
			};

		public RegistrationResponse RegistrationOK(string trueUniqueName = null)
			=> new RegistrationResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.OK,
				TrueUniqueName = trueUniqueName
			};

		public ReportResponse ReportInvalidRequest()
			=> new ReportResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.InvalidRequest
			};

		public RegistrationResponse RegistrationAcceptanceButIsAlreadyAccepted()
			=> new RegistrationResponse()
			{
				ResponseCode = (int)RegistrationResponseCodeEnum.AlreadyRegistered
			};

		public RegistrationResponse RegistrationInvalidRequest()
			=> new RegistrationResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.InvalidRequest
			};

		public ReportResponse ReportOKDoNothing()
			=> new ReportResponse()
			{
				Command = (int)CommandEnum.DoNothing,
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public ReportResponse ReportOKGetOrder()
			=> new ReportResponse()
			{
				Command = (int)CommandEnum.GetCoffeeOrder,
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public ReportResponse ReportOKConfirmDisabling()
			=> new ReportResponse()
			{
				Command = (int)CommandEnum.DisablingConfirmed,
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public ReportResponse ReportOKDisable()
			=> new ReportResponse()
			{
				Command = (int)CommandEnum.Disable,
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public OrderResponse OrderOKGiveMeAnOrder(uint orderReference, string recipeToMakeString)
			=> new OrderResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.OK,
				OrderReference = orderReference,
				Recipe = recipeToMakeString
			};

		public OrderResponse OrderInvalidRequest()
			=> new OrderResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.InvalidRequest
			};

		public OrderResponse OrderOKProcessingWilStart()
			=> new OrderResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public OrderResponse OrderOKReady()
			=> new OrderResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public OrderResponse OrderOKProblemOccurredDuringProcessing()
		{
			throw new NotImplementedException();
		}
	}
}