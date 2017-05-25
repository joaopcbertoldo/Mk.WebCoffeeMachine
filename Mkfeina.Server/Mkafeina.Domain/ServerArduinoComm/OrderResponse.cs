namespace Mkafeina.Domain.ServerArduinoComm
{
	public class OrderResponse : Response
	{
		// 0 eh invalido
		public uint OrderReference { get; set; }

		public string Recipe { get; set; }
	}
}