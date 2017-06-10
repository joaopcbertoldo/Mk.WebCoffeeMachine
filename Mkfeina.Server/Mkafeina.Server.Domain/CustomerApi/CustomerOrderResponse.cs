namespace Mkafeina.Server.Domain.CustomerApi
{
	public enum CustomerResponseCodeEnum
	{
		Undef = 0,
		InexistentMachine = 1,
		CMDisabled = 2,
		RecipeNotAvailable = 3,
		FullQueue = 4,
		OrderReceived = 5
	}

	public class CustomerOrderResponse
	{
		public string Message { get; set; }

		public CustomerResponseCodeEnum Code { get; set; }
	}
}