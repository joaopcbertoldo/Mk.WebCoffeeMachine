namespace Mkafeina.Server.Domain.CustomerApi
{
	public class CustomerOrderRequest
	{
		public string MachineUniqueName { get; set; }

		public string RecipeName { get; set; }

		public string CustomerEmail { get; set; }
	}
}