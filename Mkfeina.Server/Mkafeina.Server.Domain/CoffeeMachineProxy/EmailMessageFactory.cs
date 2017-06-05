using System;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	internal class EmailMessageFactory
	{
		private string _signature;

		public EmailMessageFactory(string signature)
		{
			_signature = signature;
		}
		internal string MakeSubject(TemplateEmailEnum message)
		{
			//var subject = $"MKafeína - Pedido ref #{_waitress.Reference} CANCELADO!";
			//var subject = $"MKafeína - Pedido ref #{_orderDequeued.Reference} pronto!";
			return "Falta implementar.";
		}

		internal string MakeMessage(TemplateEmailEnum message)
		{
			//var message = $"O seu pedido de {_orderDequeued.RecipeName} (ref #{_orderDequeued.Reference}) já pode ser retirado na MKafeína {_boss.State.UniqueName}.";
			//var message = $"O seu pedido de {_waitress.Recipe.Name} (ref #{_waitress.Reference}) não pode ser processado na MKafeína {_state.UniqueName}. Pedimos desculpas pelo inconveniente e agradecemos a compreensão.";
			return "Falta implementar.";
		}
	}
}