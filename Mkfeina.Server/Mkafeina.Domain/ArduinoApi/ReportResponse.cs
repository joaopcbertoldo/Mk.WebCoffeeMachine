namespace Mkafeina.Domain.ServerArduinoComm
{
	public enum CommandEnum
	{
		Undefined = 0,
		DoNothing = 1,
		GetCoffeeOrder = 100,
		Disable = 200,
		DisablingConfirmed = 201
	}

	public class ReportResponse : Response
	{
		public CommandEnum Command { get; set; } = CommandEnum.DoNothing;
	}
}