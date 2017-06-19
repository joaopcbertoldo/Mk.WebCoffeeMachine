using Mkafeina.Domain.ArduinoApi;

namespace Mkafeina.Domain.ServerArduinoComm
{
	public enum MessageEnum
	{
		Undef = 0,

		Registration = 1100,
		Offsets = 1200,
		Unregistration = 1300,

		Signals = 2100,
		Disabling = 2200,
		Reenable = 2300,

		GiveMeAnOrder = 3100,
		Ready = 3200,
		CancelOrders = 3300
	}

	public class ArduinoRequest
	{
		public string mac { get; set; }

		public MessageEnum msg { get; set; }
	}

	public class RegistrationRequest : ArduinoRequest
	{
		public string un { get; set; }

		public IngredientsSetup stp { get; set; }
	}

	public class ReportRequest : ArduinoRequest
	{
		public IngredientsSignals sig { get; set; }
	}

	public class OrderRequest : ArduinoRequest
	{
		public string oref { get; set; }
	}
}