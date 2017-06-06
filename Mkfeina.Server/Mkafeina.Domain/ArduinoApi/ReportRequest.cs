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
		private const int NOT_AVAILABLE = -1;

		public ReportMessageEnum ReportMessage { get; set; }

		public bool IsEnabled { get; set; }

		public float Coffee { get; set; }

		public float Water { get; set; }

		public float Milk { get; set; } 

		public float Sugar { get; set; }
	}
}