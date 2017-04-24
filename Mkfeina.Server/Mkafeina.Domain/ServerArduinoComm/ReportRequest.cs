using static Mkfeina.Domain.ServerArduinoComm.Constants;

namespace Mkfeina.Domain.ServerArduinoComm
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

		public int CoffeeLevel { get; set; } = NOT_AVAILABLE;

		public int WaterLevel { get; set; } = NOT_AVAILABLE;

		public int MilkLevel { get; set; } = NOT_AVAILABLE;

		public int SugarLevel { get; set; } = NOT_AVAILABLE;
	}
}