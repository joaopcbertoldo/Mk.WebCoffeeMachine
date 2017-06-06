namespace Mkafeina.Domain.ServerArduinoComm
{
	public enum RegistrationResponseCodeEnum
	{
		RegisteredButNotAccepted = 57,
		AlreadyRegistered = 75
	}

	public class RegistrationResponse : Response
	{
		public string TrueUniqueName { get; set; }
	}
}