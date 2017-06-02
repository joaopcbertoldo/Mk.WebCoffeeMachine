using Mkafeina.Domain;
using Mkafeina.Domain.Entities;
using Mkafeina.Domain.ServerArduinoComm;
using Mkfeina.Domain.ServerArduinoComm;
using Mkfeina.Server.Domain;
using System;
using System.Linq;

namespace Mkafeina.Server.Domain
{
	public enum RegistrationStatusEnum
	{
		Undefined = 0,
		NotRegistered,
		RegistrationNotAccepted,
		Registered
	}

	public class CMProxy
	{
		private Order _orderBuffer;

		public CMProxyState State;

#warning fazer o dash inscrever no eventoGY

		public CMProxyOffsets Offsets;

		public event Action<string> StateChangeEvent;

		private void OnStateChangeEvent()
		{
		}

		public static CMProxy CreateNew(string trueUniqueName, RegistrationRequest request)
		{
			var state = new CMProxyState()
			{
				Mac = request.Mac,
				UniqueName = trueUniqueName,
				CoffeeLevel = 0,
				WaterLevel = 0,
				MilkLevel = 0,
				SugarLevel = 0,
				RegistrationIsAccepted = false,
				IsMakingCoffee = false,
				IsEnabled = false
			};

			var offsets = new CMProxyOffsets()
			{
				CoffeeEmptyOffset = request.CoffeeEmptyOffset,
				CoffeeFullOffset = request.CoffeeFullOffset,
				WaterEmptyOffset = request.WaterEmptyOffset,
				WaterFullOffset = request.WaterFullOffset,
				MilkEmptyOffset = request.MilkEmptyOffset,
				MilkFullOffset = request.MilkFullOffset,
				SugarEmptyOffset = request.SugarEmptyOffset,
				SugarFullOffset = request.SugarFullOffset
			};

			var proxy = new CMProxy()
			{
				State = state,
				Offsets = offsets
			};

			state.StateChangeEvent += proxy.OnStateChangeEvent;
#warning arrumar esquema de atualizacao do dash e mudar esquema de criacao de painel
			//var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<Dashboard>();
			//proxy.StateChangeEvent += dash.UpdateEventHandler("log");

			return proxy;
		}

		private ArduinoResponseFactory _ardResponseFac = new ArduinoResponseFactory();

#warning TODO transform email sender creation in config
		private EmailSender _emailSender = new EmailSender("mkafeina@gmail.com", "3uqVer0c4f33!", "smtp.gmail.com", 587);

		public ReportResponse HandleReportRequest(ReportRequest request)
		{
			switch (request.ReportMessage)
			{
				case (int)ReportMessageEnum.Levels:
					State.Update(request, Offsets);

					if (State.LevelsUnderMinimum())
						return _ardResponseFac.ReportDisable();
					else if (_orderBuffer == null)
						return _ardResponseFac.ReportNoOrder();
					else
						return _ardResponseFac.ReportGetCoffeeOrder();

				case (int)ReportMessageEnum.DisablingCoffeeMachine:
					State.IsEnabled = false;
					return _ardResponseFac.ReportConfirmDisabling();

				default:
					return _ardResponseFac.ReportInvalidRequest();
			}
		}

		public OrderResponse HandleOrderRequest(OrderRequest request)
		{
			switch (request.OrderMessage)
			{
				case (int)OrderMessageEnum.GiveMeAnOrder:
					if (_orderBuffer != null && _orderBuffer.Status == OrderStatusEnum.OrderWaiting)
					{
						_orderBuffer.Status = OrderStatusEnum.OrderTaken;
						return _ardResponseFac.OrderGiverAnOrderOK(_orderBuffer.Reference, _orderBuffer.Recipe);
					}
					else
						return _ardResponseFac.OrderInvalidRequest();

				case (int)OrderMessageEnum.ProcessingWillStart:
					if (_orderBuffer != null && _orderBuffer.Status == OrderStatusEnum.OrderTaken && request.OrderReference == _orderBuffer.Reference)
					{
						_orderBuffer.Status = OrderStatusEnum.BeingProcessed;
						return _ardResponseFac.OrderProcessingWilStartOK();
					}
					else
						return _ardResponseFac.OrderInvalidRequest();

				case (int)OrderMessageEnum.OrderReady:
					if (_orderBuffer != null && _orderBuffer.Status == OrderStatusEnum.BeingProcessed && request.OrderReference == _orderBuffer.Reference)
					{
#warning parametrizar coisas do email
						var subject = $"MKafeína - Pedido ref #{_orderBuffer.Reference} pronto!";
						var message = $"O seu pedido de {_orderBuffer.Recipe.Name} (ref #{_orderBuffer.Reference}) já pode ser retirado na MKafeína {State.UniqueName}.";
						_emailSender.SendMail(subject, message, _orderBuffer.CustomerEmail);
						_orderBuffer = null;
						return _ardResponseFac.OrderReadyOK();
					}
					else
						return _ardResponseFac.OrderInvalidRequest();

				case (int)OrderMessageEnum.ProblemOcurredDuringProcessing:
					if (_orderBuffer != null && _orderBuffer.Status == OrderStatusEnum.BeingProcessed && request.OrderReference == _orderBuffer.Reference)
					{
#warning parametrizar coisas do email
						var subject = $"MKafeína - Pedido ref #{_orderBuffer.Reference} CANCELADO!";
						var message = $"O seu pedido de {_orderBuffer.Recipe.Name} (ref #{_orderBuffer.Reference}) não pode ser processado na MKafeína {State.UniqueName}. Pedimos desculpas pelo inconveniente e agradecemos a compreensão.";
						_emailSender.SendMail(subject, message, _orderBuffer.CustomerEmail);
						_orderBuffer = null;
						return _ardResponseFac.OrderReadyOK();
					}
					else
						return _ardResponseFac.OrderInvalidRequest();

				default:
					return _ardResponseFac.OrderInvalidRequest();
			}
		}

		public CustomerOrderResponse HandleClientOrderRequest(CustomerOrderRequest request)
		{
#warning trasnformar minimos em configs
			if (State.IsEnabled)
			{
				//if (!CookBook.Sgt.AllRecipesNames.Contains(request.RecipeName))
				//	return new CustomerOrderResponse()
				//	{
				//		Message = "Recipe not known."
				//	};
				//
				//_orderBuffer = new Order()
				//{
				//	CustomerEmail = request.Email,
				//	Recipe = CookBook.Sgt.AllRecipes.First(kv => kv.Key == request.RecipeName).Value,
				//	Reference = (uint)new Random((int)DateTime.Now.ToBinary()).Next(),
				//	Status = OrderStatusEnum.OrderWaiting
				//};
				//
				//return new CustomerOrderResponse()
				//{
				//	Message = $"Ok, your order will be processed and we will send you a message at {request.Email} when it is ready."
				//};
			}
			//else
				return new CustomerOrderResponse()
				{
					Message = "Coffee Machine is currently disabled."
				};
		}
	}
}