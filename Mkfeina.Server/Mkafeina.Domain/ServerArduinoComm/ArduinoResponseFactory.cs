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
	}
}