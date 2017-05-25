using static Mkafeina.Domain.ServerArduinoComm.Constants;

namespace Mkafeina.Domain.ServerArduinoComm
{
	public enum ReportMessageEnum
	{
		Undefined = 0,
		Levels = 100,
		DisablingCoffeeMachine = 900
	}

	public class ReportRequest : Request
	{
		public int ReportMessage { get; set; }

		public bool IsEnabled { get; set; }

		public float CoffeeLevel { get; set; } = NOT_AVAILABLE;

		public float WaterLevel { get; set; } = NOT_AVAILABLE;

		public float MilkLevel { get; set; } = NOT_AVAILABLE;

		public float SugarLevel { get; set; } = NOT_AVAILABLE;
	}
}