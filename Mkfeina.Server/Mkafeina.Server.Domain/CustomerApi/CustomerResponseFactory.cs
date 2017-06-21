namespace Mkafeina.Server.Domain.CustomerApi
{
	public class CustomerResponseFactory
	{
		public CustomerOrderResponse InexistentCoffeeMachine()
			=> new CustomerOrderResponse()
			{
				Code = CustomerResponseCodeEnum.InexistentMachine,
				Message = "Error: inexistent coffee machine."
			};

		public CustomerOrderResponse CMCurrentlyDisabled()
			=> new CustomerOrderResponse()
			{
				Code = CustomerResponseCodeEnum.CMDisabled,
				Message = "Error: coffee machine is currently disabled."
			};

		public CustomerOrderResponse RecipeNotAvailable()
			=> new CustomerOrderResponse()
			{
				Code = CustomerResponseCodeEnum.RecipeNotAvailable,
				Message = "Error: recipe not available."
			};

		internal CustomerOrderResponse FullQueue()
			=> new CustomerOrderResponse()
			{
				Code = CustomerResponseCodeEnum.FullQueue,
				Message = "Error: queue is full, wate some moments to make your order."
			};

		internal CustomerOrderResponse OrderReceived(int positionInQueue, string email)
			=> new CustomerOrderResponse()
			{
				Code = CustomerResponseCodeEnum.OrderReceived,
				Message = $"Pedido recebido! Você é o número {positionInQueue} na fila. " +
						  (positionInQueue == 1 ? $"Você será notificado pelo e-mails <<{email}>> quando seu pedido estiver pronto." :
												  $"Você será notificado pelo e-mails <<{email}>> quando seu pedido for processado.")
			};

		private string Ordinal(int positionInQueue)
		{
			switch (positionInQueue)
			{
				case 1: return "1st";
				case 2: return "2nd";
				case 3: return "3rd";
				case 4: return "4th";
				case 5: return "5th";
				default:
					return "???";
			}
		}
	}
}