using Mkfeina.Domain;
using Mkfeina.Domain.ServerArduinoComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mkfeina.Server.Domain
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

	public class CoffeeMachineProxy
	{
		private EmailSender _emailSender = new EmailSender("mkafeina@gmail.com");

		private static Dictionary<string, CoffeeMachineProxy> _coffeeMachines = new Dictionary<string, CoffeeMachineProxy>();

		public static RegistrationStatusEnum RegistrationStatus(string mac)
		{
			if (!_coffeeMachines.ContainsKey(mac))
				return RegistrationStatusEnum.NotRegistered;
			else if (!_coffeeMachines[mac].RegistrationIsAccepted)
				return RegistrationStatusEnum.RegistrationNotAccepted;
			else
				return RegistrationStatusEnum.Registered;
		}

		public static bool MachineIsRegistered(string uniqueName)
			=> _coffeeMachines.Any(kv => kv.Value.UniqueName == uniqueName) &&
			   _coffeeMachines.First(kv => kv.Value.UniqueName == uniqueName).Value.RegistrationIsAccepted;

		public static CoffeeMachineProxy GetProxy(string mac)
		{
			if (!_coffeeMachines.ContainsKey(mac))
				return null;
			return _coffeeMachines[mac];
		}

		public static CoffeeMachineProxy GetProxyByUniqueName(string uniqueName)
			=> _coffeeMachines.FirstOrDefault(kv => kv.Value.UniqueName == uniqueName).Value;

		public static RegistrationResponse HandleRegistrationAttempt(RegistrationRequest request)
		{
			lock (_coffeeMachines)
			{
				var mac = request.Mac;
				if (_coffeeMachines.ContainsKey(mac))
					return new RegistrationResponse()
					{
						ResponseCode = _coffeeMachines[mac].RegistrationIsAccepted ? (int)RegistrationResponseCodeEnum.AlreadyRegistered :
																					 (int)RegistrationResponseCodeEnum.RegisteredButNotAccepted
					};

				string trueUniqueName = request.UniqueName;
				while (_coffeeMachines.Any(kv => kv.Value.UniqueName == request.UniqueName))
					trueUniqueName = trueUniqueName.GenerateNameVersion();

				var newProxy = new CoffeeMachineProxy()
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
					Offsets = new CoffeeMachineProxyOffsets()
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

				_coffeeMachines.Add(mac, newProxy);

				return new RegistrationResponse()
				{
					ResponseCode = (int)ResponseCodeEnum.OK,
					TrueUniqueName = trueUniqueName
				};
			}
		}

		public static RegistrationResponse HandleRegistrationAcceptance(RegistrationRequest request)
		{
			lock (_coffeeMachines)
			{
				if (_coffeeMachines.ContainsKey(request.Mac) && !_coffeeMachines[request.Mac].RegistrationIsAccepted)
				{
					_coffeeMachines[request.Mac].RegistrationIsAccepted = true;
					return new RegistrationResponse()
					{
						ResponseCode = (int)ResponseCodeEnum.OK
					};
				}
				else if (_coffeeMachines.ContainsKey(request.Mac) && _coffeeMachines[request.Mac].RegistrationIsAccepted)
					return new RegistrationResponse()
					{
						ResponseCode = (int)RegistrationResponseCodeEnum.AlreadyRegistered
					};
				else
					return new RegistrationResponse()
					{
						ResponseCode = (int)ResponseCodeEnum.InvalidRequest
					};
			}
		}

		public ReportResponse HandleReportRequest(ReportRequest request)
		{
			if (request.ReportMessage == (int)ReportMessageEnum.Levels)
			{
				CoffeeLevel = (int)((request.CoffeeLevel - Offsets.CoffeeEmptyOffset) / (Offsets.CoffeeFullOffset - Offsets.CoffeeEmptyOffset) * 100);
				WaterLevel = (int)((request.WaterLevel - Offsets.WaterEmptyOffset) / (Offsets.WaterFullOffset - Offsets.WaterEmptyOffset) * 100);
				SugarLevel = (int)((request.SugarLevel - Offsets.SugarEmptyOffset) / (Offsets.SugarFullOffset - Offsets.SugarEmptyOffset) * 100);
				MilkLevel = (int)((request.MilkLevel - Offsets.MilkEmptyOffset) / (Offsets.MilkFullOffset - Offsets.MilkEmptyOffset) * 100);
				IsEnabled = request.IsEnabled;

				if (_orderState == OrderStateEnum.NoOrder)
					return new ReportResponse()
					{
						Command = (int)CommandEnum.DoNothing,
						ResponseCode = (int)ResponseCodeEnum.OK
					};
				else
				{
					return new ReportResponse()
					{
						Command = (int)CommandEnum.GetCoffeeOrder,
						ResponseCode = (int)ResponseCodeEnum.OK
					};
				}
			}
			else if (request.ReportMessage == (int)ReportMessageEnum.DisablingCoffeeMachine)
			{
				IsEnabled = false;
				return new ReportResponse()
				{
					Command = (int)CommandEnum.DisablingConfirmed,
					ResponseCode = (int)ResponseCodeEnum.OK
				};
			}
			else
			{
				return new ReportResponse()
				{
					ResponseCode = (int)ResponseCodeEnum.InvalidRequest
				};
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
				if (!CookBook.Singleton.AllRecipesNames.Contains(request.RecipeName))
					return new CustomerOrderResponse()
					{
						Message = "Recipe not known."
					};

				_orderReference = (uint)new Random((int)DateTime.Now.ToBinary()).Next();
				_recipeToMake = CookBook.Singleton.AllRecipes.First(kv => kv.Key == request.RecipeName).Value;
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

		private CoffeeMachineProxyState _internalState = new CoffeeMachineProxyState();

		public event Action<CoffeeMachineProxy> StatusChangeEvent;

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

		public CoffeeMachineProxyOffsets Offsets = new CoffeeMachineProxyOffsets();

		public MakeCoffeeResponseEnum MakeCoffee(string recipe)
		{
			return MakeCoffeeResponseEnum.Undefined;
		}
	}
}