using Mkafeina.Domain.ArduinoApi;

namespace Mkafeina.Domain.ServerArduinoComm
{
	public enum ResponseCodeEnum
	{
		Undef = 0,
		OK = 200,
		InvalidRequest = 400,
		InternalServerError = 401
	}

	public enum CommandEnum
	{
		Undef = 0,
		Void = 100,
		Disable = 200,
		Process = 300,
		Enable = 400,
		Register = 500,
		TakeAnOrder = 501
	}

	public enum ErrorEnum
	{
		Undef = 0,
		MacAlreadyRegistered = 75,
		Void = 76,
		MacNotRegistered = 77,
		UnknownMessage = 78,
		MissingIngredientsSetup = 79,
		ShouldNotSentOffsets = 80,
		MachineDisabledCannotTakeOrders = 81,
		MachineAskedForOrderButThereIsNone = 82,
		OrderAlreadyTaken = 83,
		ShouldNotBeProcessing = 84,
		ShouldBeAlreadyEnabled = 85,
		WrongOrderReference = 86,
		ShouldNotSendSignals = 87,
		DisabledWithoutWarning = 88
	}

	public class ArduinoResponse
	{
		public ResponseCodeEnum ResponseCode { get; set; }

		public CommandEnum Command { get; set; }

		public ErrorEnum Error { get; set; }
	}

	public class ReportResponse : ArduinoResponse
	{
	}

	public class RegistrationResponse : ArduinoResponse
	{
	}

	public class OrderResponse : ArduinoResponse
	{
		public string OrderReference { get; set; }

		// OLD VERSION
		//public string Recipe { get; set; }
		public RecipeObj Recipe { get; set; }
	}
}