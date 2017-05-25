using Mkafeina.Domain;
using Mkafeina.Domain.ServerArduinoComm;
using Mkfeina.Domain.ServerArduinoComm;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mkafeina.Server.Domain
{
	public enum RegistrationStatusEnum
	{
		Undefined = 0,
		NotRegistered,
		RegistrationNotAccepted,
		Registered
	}

	public enum OrderStateEnum
	{
		NoOrder = 0,
		OrderArrived,
		OrderTaken,
		BeingProcessed
	}

	public class CMProxy
	{
		private ArduinoResponseFactory _ardResponseFac = new ArduinoResponseFactory();

		private EmailSender _emailSender = new EmailSender("mkafeina@gmail.com");

		public ReportResponse HandleReportRequest(ReportRequest request)
		{
			switch (request.ReportMessage)
			{
				case (int)ReportMessageEnum.Levels:
					//CoffeeLevel = (int)((request.CoffeeLevel - Offsets.CoffeeEmptyOffset) / (Offsets.CoffeeFullOffset - Offsets.CoffeeEmptyOffset) * 100);
					//WaterLevel = (int)((request.WaterLevel - Offsets.WaterEmptyOffset) / (Offsets.WaterFullOffset - Offsets.WaterEmptyOffset) * 100);
					//SugarLevel = (int)((request.SugarLevel - Offsets.SugarEmptyOffset) / (Offsets.SugarFullOffset - Offsets.SugarEmptyOffset) * 100);
					//MilkLevel = (int)((request.MilkLevel - Offsets.MilkEmptyOffset) / (Offsets.MilkFullOffset - Offsets.MilkEmptyOffset) * 100);
					CoffeeLevel = CMProxyOffsets.AdjustSignal(request.CoffeeLevel, "Coffee");
					WaterLevel = CMProxyOffsets.AdjustSignal(request.WaterLevel, "Water");
					SugarLevel = CMProxyOffsets.AdjustSignal(request.SugarLevel, "Sugar");
					MilkLevel = CMProxyOffsets.AdjustSignal(request.MilkLevel, "Milk");
					IsEnabled = request.IsEnabled;

					if (_orderState == OrderStateEnum.NoOrder)
						return _ardResponseFac.ReportNoOrder();
					else
						return _ardResponseFac.ReportGetCoffeeOrder();

				case (int)ReportMessageEnum.DisablingCoffeeMachine:
					IsEnabled = false;
					return _ardResponseFac.ReportConfirmDisabling();

				default:
					return _ardResponseFac.ReportInvalidRequest();
			}
		}

		public OrderResponse HandleOrderRequest(OrderRequest request)
		{
			if (request.OrderMessage == (int)OrderMessageEnum.GiveMeAnOrder)
			{
				if (_orderState == OrderStateEnum.OrderArrived)
				{
					_orderState = OrderStateEnum.OrderTaken;
					return new OrderResponse()
					{
						ResponseCode = (int)ResponseCodeEnum.OK,
						OrderReference = _orderReference,
						Recipe = _recipeToMake.ToString()
					};
				}
				else
					return new OrderResponse()
					{
						ResponseCode = (int)ResponseCodeEnum.InvalidRequest
					};
			}
			else if (request.OrderMessage == (int)OrderMessageEnum.ProcessingWillStart)
			{
				if (_orderState == OrderStateEnum.OrderTaken && request.OrderReference == _orderReference)
				{
					_orderState = OrderStateEnum.BeingProcessed;
					return new OrderResponse()
					{
						ResponseCode = (int)ResponseCodeEnum.OK
					};
				}
				else
					return new OrderResponse()
					{
						ResponseCode = (int)ResponseCodeEnum.InvalidRequest
					};
			}
			else if (request.OrderMessage == (int)OrderMessageEnum.OrderReady ||
					 request.OrderMessage == (int)OrderMessageEnum.ProblemOcurredDuringProcessing)
			{
				if (_orderState == OrderStateEnum.BeingProcessed && request.OrderReference == _orderReference)
				{
					var recipeName = _recipeToMake.Name;
					var email = _email;
					Task.Run(() =>
					{
						_emailSender.SendMail($"Your <<{recipeName}>> is ready!", email);
					});

					_orderState = OrderStateEnum.NoOrder;
					_recipeToMake = null;
					_email = "";
					_orderReference = 0;

					return new OrderResponse()
					{
						ResponseCode = (int)ResponseCodeEnum.OK
					};
				}
				else
					return new OrderResponse()
					{
						ResponseCode = (int)ResponseCodeEnum.InvalidRequest
					};
			}
			else
				return new OrderResponse()
				{
					ResponseCode = (int)ResponseCodeEnum.InvalidRequest
				};
		}

		public CustomerOrderResponse HandleClientOrderRequest(CustomerOrderRequest request)
		{
			if (CoffeeLevel >= 10 && WaterLevel >= 10)
			{
				if (!CookBook.Sgt.AllRecipesNames.Contains(request.RecipeName))
					return new CustomerOrderResponse()
					{
						Message = "Recipe not known."
					};

				_orderReference = (uint)new Random((int)DateTime.Now.ToBinary()).Next();
				_recipeToMake = CookBook.Sgt.AllRecipes.First(kv => kv.Key == request.RecipeName).Value;
				_orderState = OrderStateEnum.OrderArrived;
				_email = request.Email;

				return new CustomerOrderResponse()
				{
					Message = $"Ok, your order will be processed and we will send you a message at {request.Email} when it is ready."
				};
			}
			else
				return new CustomerOrderResponse()
				{
					Message = "Not enough ingredients, please report SAdEM."
				};
		}

		private OrderStateEnum _orderState;

		private string _email;

		private uint _orderReference;

		private Recipe _recipeToMake;

		private CMProxyState _internalState = new CMProxyState();

		public event Action<CMProxy> StatusChangeEvent;

#warning fazer o dash inscrever no eventoGY

		public string UniqueName {
			get => _internalState.UniqueName;
			set {
				_internalState.UniqueName = value;
				StatusChangeEvent?.Invoke(this);
			}
		}

		public bool IsMakingCoffee {
			get => _internalState.IsMakingCoffee;
			set {
				_internalState.IsMakingCoffee = value;
				StatusChangeEvent?.Invoke(this);
			}
		}

		public int CoffeeLevel {
			get => _internalState.CoffeeLevel;
			set {
				_internalState.CoffeeLevel = value;
				StatusChangeEvent?.Invoke(this);
			}
		}

		public int WaterLevel {
			get => _internalState.WaterLevel;
			set {
				_internalState.WaterLevel = value;
				StatusChangeEvent?.Invoke(this);
			}
		}

		public int MilkLevel {
			get => _internalState.MilkLevel;
			set {
				_internalState.MilkLevel = value;
				StatusChangeEvent?.Invoke(this);
			}
		}

		public int SugarLevel {
			get => _internalState.SugarLevel;
			set {
				_internalState.SugarLevel = value;
				StatusChangeEvent?.Invoke(this);
			}
		}

		public bool RegistrationIsAccepted {
			get => _internalState.RegistrationIsAccepted;
			set {
				_internalState.RegistrationIsAccepted = value;
				StatusChangeEvent?.Invoke(this);
			}
		}

		public bool IsEnabled {
			get => _internalState.IsEnabled;
			set {
				_internalState.IsEnabled = value;
				StatusChangeEvent?.Invoke(this);
			}
		}

		public string Mac { get; set; }

		public CMProxyOffsets Offsets = new CMProxyOffsets();

		public MakeCoffeeResponseEnum MakeCoffee(string recipe)
		{
			return MakeCoffeeResponseEnum.Undefined;
		}

		public static CMProxy CreateNew(string trueUniqueName, RegistrationRequest request)
			=> new CMProxy()
			{
				Mac = request.Mac,
				UniqueName = trueUniqueName,
				CoffeeLevel = 0,
				WaterLevel = 0,
				MilkLevel = 0,
				SugarLevel = 0,
				RegistrationIsAccepted = false,
				IsMakingCoffee = false,
				IsEnabled = false,
				Offsets = new CMProxyOffsets()
				{
					CoffeeEmptyOffset = request.CoffeeEmptyOffset,
					CoffeeFullOffset = request.CoffeeFullOffset,
					WaterEmptyOffset = request.WaterEmptyOffset,
					WaterFullOffset = request.WaterFullOffset,
					MilkEmptyOffset = request.MilkEmptyOffset,
					MilkFullOffset = request.MilkFullOffset,
					SugarEmptyOffset = request.SugarEmptyOffset,
					SugarFullOffset = request.SugarFullOffset
				}
			};
	}
}