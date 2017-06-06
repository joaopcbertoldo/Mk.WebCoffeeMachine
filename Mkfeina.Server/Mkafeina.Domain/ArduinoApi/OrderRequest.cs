namespace Mkafeina.Domain.ServerArduinoComm
{
	public enum OrderMessageEnum
	{
		Undefined = 0,
		GiveMeAnOrder = 100,
		ProcessingWillStart = 200,
		OrderReady = 300,
		ProblemOcurredDuringProcessing = 400
	}

	public class OrderRequest : Request
	{
		public OrderMessageEnum OrderMessage { get; set; }

		public uint OrderReference { get; set; }
	}
}