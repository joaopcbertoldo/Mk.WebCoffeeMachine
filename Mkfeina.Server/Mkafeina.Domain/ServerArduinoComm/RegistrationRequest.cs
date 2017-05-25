using static Mkafeina.Domain.ServerArduinoComm.Constants;

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
		public int RegistrationMessage { get; set; }

		public string UniqueName { get; set; }

		public int CoffeeEmptyOffset { get; set; } = NOT_AVAILABLE;

		public int CoffeeFullOffset { get; set; } = NOT_AVAILABLE;

		public int WaterEmptyOffset { get; set; } = NOT_AVAILABLE;

		public int WaterFullOffset { get; set; } = NOT_AVAILABLE;

		public int MilkEmptyOffset { get; set; } = NOT_AVAILABLE;

		public int MilkFullOffset { get; set; } = NOT_AVAILABLE;

		public int SugarEmptyOffset { get; set; } = NOT_AVAILABLE;

		public int SugarFullOffset { get; set; } = NOT_AVAILABLE;
	}
}