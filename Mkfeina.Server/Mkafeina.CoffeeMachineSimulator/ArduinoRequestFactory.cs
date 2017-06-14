using System;
using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Simulator;

namespace Mkafeina.CoffeeMachineSimulator
{
	internal class ArduinoRequestFactory
	{
		private FakeCoffeMachine _fakeCoffeMachine;

		public ArduinoRequestFactory(FakeCoffeMachine fakeCoffeMachine)
		{
			_fakeCoffeMachine = fakeCoffeMachine;
		}

		internal RegistrationRequest Registration()
			=> new RegistrationRequest()
			{
				Msg = MessageEnum.Registration,
				Mac = _fakeCoffeMachine.Mac,
				UniqueName = _fakeCoffeMachine.UniqueName,
				IngredientsSetup = new IngredientsSetup()
				{
					CoffeeAvailable = true,
					CoffeeEmptyOffset = _fakeCoffeMachine.Signals.CoffeeMin,
					CoffeeFullOffset = _fakeCoffeMachine.Signals.CoffeeMax,

					SugarAvailable = true,
					SugarEmptyOffset = _fakeCoffeMachine.Signals.SugarMin,
					SugarFullOffset = _fakeCoffeMachine.Signals.SugarMax,

					WaterAvailable = true,
					WaterEmptyOffset = _fakeCoffeMachine.Signals.WaterMin,
					WaterFullOffset = _fakeCoffeMachine.Signals.WaterMax
				}
			};

		internal RegistrationRequest Offsets()
			=> new RegistrationRequest()
			{
				Msg = MessageEnum.Offsets,
				Mac = _fakeCoffeMachine.Mac,
				IngredientsSetup = new IngredientsSetup()
				{
					CoffeeAvailable = true,
					CoffeeEmptyOffset = _fakeCoffeMachine.Signals.CoffeeMin,
					CoffeeFullOffset = _fakeCoffeMachine.Signals.CoffeeMax,

					SugarAvailable = true,
					SugarEmptyOffset = _fakeCoffeMachine.Signals.SugarMin,
					SugarFullOffset = _fakeCoffeMachine.Signals.SugarMax,

					WaterAvailable = true,
					WaterEmptyOffset = _fakeCoffeMachine.Signals.WaterMin,
					WaterFullOffset = _fakeCoffeMachine.Signals.WaterMax
				}
			};

		internal RegistrationRequest Unregistration()
			=> new RegistrationRequest()
			{
				Msg = MessageEnum.Unregistration,
				Mac = _fakeCoffeMachine.Mac
			};

		internal ReportRequest Signals()
			=> new ReportRequest()
			{
				Msg = MessageEnum.Signals,
				Mac = _fakeCoffeMachine.Mac,
				Signals = new IngredientsSignals()
				{
					Coffee = _fakeCoffeMachine.Signals.Coffee,
					Water = _fakeCoffeMachine.Signals.Water,
					Sugar = _fakeCoffeMachine.Signals.Sugar,
					Enabled = _fakeCoffeMachine.Signals.Enabled
				}
			};

		internal ReportRequest Disabling()
			=> new ReportRequest()
			{
				Msg = MessageEnum.Disabling,
				Mac = _fakeCoffeMachine.Mac
			};

		internal ReportRequest Reenable()
			=> new ReportRequest()
			{
				Mac = _fakeCoffeMachine.Mac,
				Msg = MessageEnum.Reenable
			};

		internal OrderRequest GiveMeAnOrder()
			=> new OrderRequest()
			{
				Mac = _fakeCoffeMachine.Mac,
				Msg = MessageEnum.GiveMeAnOrder
			};

		internal OrderRequest OrderReady(string orderReference)
			=> new OrderRequest()
			{
				Mac = _fakeCoffeMachine.Mac,
				Msg = MessageEnum.Ready,
				OrderReference = orderReference
			};

		internal OrderRequest CancelOrders()
			=> new OrderRequest()
			{
				Mac = _fakeCoffeMachine.Mac,
				Msg = MessageEnum.CancelOrders
			};
	}
}