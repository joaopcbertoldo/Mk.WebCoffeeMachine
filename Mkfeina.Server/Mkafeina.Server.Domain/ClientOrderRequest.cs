namespace Mkfeina.Server.Domain
{
	public class CustomerOrderRequest
	{
		public string MachineUniqueName { get; set; }

		public string RecipeName { get; set; }

		public string Email { get; set; }
	}
}