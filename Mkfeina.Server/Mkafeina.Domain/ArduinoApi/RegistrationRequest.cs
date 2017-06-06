using Mkafeina.Domain.ArduinoApi;

namespace Mkafeina.Domain.ServerArduinoComm
{
	public enum RegistrationMessageEnum
	{
		Undefined = 0,
		AttemptRegistration = 100,
		RegistrationAcceptance = 101,
		Offsets = 200,
		Unregister = 300
	}

	public class RegistrationRequest : Request
	{
		public const int NOT_AVAILABLE = -1;

		public RegistrationMessageEnum RegistrationMessage { get; set; }

		public string UniqueName { get; set; }

		public IngredientsSetup IngredientsSetup { get; set; }
	}
}