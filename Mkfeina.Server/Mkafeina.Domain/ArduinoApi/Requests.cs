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
#warning check if mac can be seen from the http request itself
		public string Mac { get; set; }

		public MessageEnum Msg { get; set; }
	}

	public class RegistrationRequest : ArduinoRequest
	{
		public string UniqueName { get; set; }

		public IngredientsSetup IngredientsSetup { get; set; }
	}

	public class ReportRequest : ArduinoRequest
	{
		public IngredientsSignals Signals { get; set; }
	}

	public class OrderRequest : ArduinoRequest
	{
		public string OrderReference { get; set; }
	}
}