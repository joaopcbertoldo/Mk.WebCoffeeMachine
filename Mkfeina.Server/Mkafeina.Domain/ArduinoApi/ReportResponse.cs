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
		public int Command { get; set; } = (int)CommandEnum.DoNothing;
	}
}