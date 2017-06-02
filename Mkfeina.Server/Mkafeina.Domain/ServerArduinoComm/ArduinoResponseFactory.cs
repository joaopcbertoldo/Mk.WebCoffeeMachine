using Mkafeina.Domain;
using Mkafeina.Domain.Entities;
using Mkafeina.Domain.ServerArduinoComm;

namespace Mkfeina.Domain.ServerArduinoComm
{
	public class ArduinoResponseFactory
	{
		public RegistrationResponse RegistrationAttemptWithMacAlreadyExisting(bool alreadyRegistered)
			=> new RegistrationResponse()
			{
				ResponseCode = alreadyRegistered ? (int)RegistrationResponseCodeEnum.AlreadyRegistered :
												   (int)RegistrationResponseCodeEnum.RegisteredButNotAccepted
			};

		public RegistrationResponse RegistrationAttemptOK(string trueUniqueName)
			=> new RegistrationResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.OK,
				TrueUniqueName = trueUniqueName
			};

		public RegistrationResponse RegistrationAcceptanceOK()
			=> new RegistrationResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.OK
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

		public ReportResponse ReportNoOrder()
			=> new ReportResponse()
			{
				Command = (int)CommandEnum.DoNothing,
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public ReportResponse ReportGetCoffeeOrder()
			=> new ReportResponse()
			{
				Command = (int)CommandEnum.GetCoffeeOrder,
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public ReportResponse ReportConfirmDisabling()
			=> new ReportResponse()
			{
				Command = (int)CommandEnum.DisablingConfirmed,
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public ReportResponse ReportDisable()
			=> new ReportResponse()
			{
				Command = (int)CommandEnum.Disable,
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public OrderResponse OrderGiverAnOrderOK(uint orderReference, Recipe recipeToMake)
			=> new OrderResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.OK,
				OrderReference = orderReference,
				Recipe = recipeToMake.ToString()
			};

		public OrderResponse OrderInvalidRequest()
			=> new OrderResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.InvalidRequest
			};

		public OrderResponse OrderProcessingWilStartOK()
			=> new OrderResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.OK
			};

		public OrderResponse OrderReadyOK()
			=> new OrderResponse()
			{
				ResponseCode = (int)ResponseCodeEnum.OK
			};
	}
}