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
				msg = MessageEnum.Registration,
				mac = _fakeCoffeMachine.Mac,
				un = _fakeCoffeMachine.UniqueName,
				stp = new IngredientsSetup()
				{
					ca = true,
					ce = _fakeCoffeMachine.Signals.CoffeeMin,
					cf = _fakeCoffeMachine.Signals.CoffeeMax,

					sa = true,
					se = _fakeCoffeMachine.Signals.SugarMin,
					sf = _fakeCoffeMachine.Signals.SugarMax,

					wa = true,
					we = _fakeCoffeMachine.Signals.WaterMin,
					wf = _fakeCoffeMachine.Signals.WaterMax
				}
			};

		internal RegistrationRequest Offsets()
			=> new RegistrationRequest()
			{
				msg = MessageEnum.Offsets,
				mac = _fakeCoffeMachine.Mac,
				stp = new IngredientsSetup()
				{
					ca = true,
					ce = _fakeCoffeMachine.Signals.CoffeeMin,
					cf = _fakeCoffeMachine.Signals.CoffeeMax,

					sa = true,
					se = _fakeCoffeMachine.Signals.SugarMin,
					sf = _fakeCoffeMachine.Signals.SugarMax,

					wa = true,
					we = _fakeCoffeMachine.Signals.WaterMin,
					wf = _fakeCoffeMachine.Signals.WaterMax
				}
			};

		internal RegistrationRequest Unregistration()
			=> new RegistrationRequest()
			{
				msg = MessageEnum.Unregistration,
				mac = _fakeCoffeMachine.Mac
			};

		internal ReportRequest Signals()
			=> new ReportRequest()
			{
				msg = MessageEnum.Signals,
				mac = _fakeCoffeMachine.Mac,
				sig = new IngredientsSignals()
				{
					c = _fakeCoffeMachine.Signals.Coffee,
					w = _fakeCoffeMachine.Signals.Water,
					s = _fakeCoffeMachine.Signals.Sugar,
					e = _fakeCoffeMachine.Signals.Enabled
				}
			};

		internal ReportRequest Disabling()
			=> new ReportRequest()
			{
				msg = MessageEnum.Disabling,
				mac = _fakeCoffeMachine.Mac
			};

		internal ReportRequest Reenable()
			=> new ReportRequest()
			{
				mac = _fakeCoffeMachine.Mac,
				msg = MessageEnum.Reenable
			};

		internal OrderRequest GiveMeAnOrder()
			=> new OrderRequest()
			{
				mac = _fakeCoffeMachine.Mac,
				msg = MessageEnum.GiveMeAnOrder
			};

		internal OrderRequest OrderReady(string orderReference)
			=> new OrderRequest()
			{
				mac = _fakeCoffeMachine.Mac,
				msg = MessageEnum.Ready,
				oref = orderReference
			};

		internal OrderRequest CancelOrders()
			=> new OrderRequest()
			{
				mac = _fakeCoffeMachine.Mac,
				msg = MessageEnum.CancelOrders
			};
	}
}