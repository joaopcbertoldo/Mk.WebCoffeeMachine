using Microsoft.Practices.Unity;
using Mkafeina.CoffeeMachineSimulator;
using Mkafeina.Domain;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mkafeina.Simulator
{
	public class FakeCoffeMachine
	{
		private ServerCaller _serverCaller;

		internal string Mac { get; set; }

		internal string UniqueName { get; set; }

		internal CMSignals Signals { get; set; }

		internal CommandEnum _command;

		internal string _orderRef;

		internal Recipe _recipe = null;

		private CancellationTokenSource _mainTaksCancelTokenSource;

		private Task _mainTask;

		internal bool _reenableFlag;

		#region Singleton Stuff

		private static object _singletonSync = new object();

		private static FakeCoffeMachine __sgt;
		private string _recipeStr;
		private AppConfig _appconfig;

		public static FakeCoffeMachine Sgt {
			get {
				if (__sgt == null)
				{
					lock (_singletonSync)
					{
						if (__sgt == null)
							__sgt = new FakeCoffeMachine();
					}
				}
				return __sgt;
			}
		}

		private FakeCoffeMachine()
		{
			_appconfig = (AppConfig)AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();
			_serverCaller = new ServerCaller(this);
			UniqueName = _appconfig.SimulatorUniqueName;
			Mac = _appconfig.SimulatorMac;
			Signals = new CMSignals();
			_mainTaksCancelTokenSource = new CancellationTokenSource();
			_mainTask = CreateMainTask();
		}

		#endregion Singleton Stuff

		public void TurnOn()
		{
			if (_mainTask.Status == TaskStatus.Running)
				return;
			_serverCaller.RegisterNoMatterWhat();
			_mainTask.Start();
			Dashboard.Sgt.LogAsync("Main task started.");
		}

		public void TurnOff()
		{
			if (_mainTask.Status != TaskStatus.Running)
				return;
			_mainTaksCancelTokenSource.Cancel();
			Dashboard.Sgt.LogAsync("Main task stoped.");
			for (var i = 0; i < 20; i++)
			{
				if (!_serverCaller.TryToUnregister())
					break;
			}
		}

		private Task CreateMainTask()
			=> new Task(() =>
			   {
				   while (true)
				   {
					   if (_mainTaksCancelTokenSource.Token.IsCancellationRequested)
						   return;

					   _serverCaller.TryToReportSignals(out _command);

					   if (Signals.Enabled)
					   {
						   switch (_command)
						   {
							   case CommandEnum.Void:
								   Thread.Sleep(2500);
								   break;

							   case CommandEnum.Disable:
								   Signals.Enabled = false;
								   continue;

							   default:
								   break;
						   }

						   if (_command == CommandEnum.TakeAnOrder || _command == CommandEnum.Process)
						   {
							   for (var i = 0; i < 20; i++) { if (_serverCaller.TryToTakeOrder(out _command, out _orderRef, out _recipeStr)) break; }
							   if (_command == CommandEnum.Process)
							   {
								   var ack = MakeCoffee(Recipe.Parse(_recipeStr));
								   if (ack)
								   {
									   for (var i = 0; i < 20; i++) { if (_serverCaller.TryReady(out _command)) break; }
								   }
								   else
								   {
									   for (var i = 0; i < 20; i++) { if (_serverCaller.TryCancelOrders(out _command)) break; }
								   }
							   }
						   }
					   }
					   else if (_reenableFlag)
					   {
						   while (!_serverCaller.TryToReenable(out _command))
						   { }
						   if (_command == CommandEnum.Enable)
							   Signals.Enabled = true;
					   }
				   }
			   }, _mainTaksCancelTokenSource.Token);

		public bool MakeCoffee(Recipe recipe)
		{
			try
			{
				lock (this)
				{
					Signals.MakingCoffee = true;

					var coffeeMeasures = recipe["Coffee"] * 0.15;
					var originalCoffeeLevel = Signals.Coffee;

					var waterMl = (Signals.CoffeeMax - Signals.CoffeeMin) * recipe["Water"]/1000;
					var originalWaterLevel = Signals.Water;

					var sugarMeasures = recipe["Sugar"] * 0.15;
					var originalSugarLevel = Signals.Sugar;

					Dashboard.Sgt.LogAsync($"Adding coffee.");
					while (Signals.Coffee > originalCoffeeLevel - 0.15)
					{
						Thread.Sleep(_appconfig.IngredientAdditionDelayMs);
						Signals.Coffee -= (float)0.01;
					}

					Dashboard.Sgt.LogAsync($"Adding water.");
					while (Signals.Water > originalWaterLevel - 0.15)
					{
						Thread.Sleep(_appconfig.IngredientAdditionDelayMs);
						Signals.Water -= (float)0.01;
					}

					Dashboard.Sgt.LogAsync($"Adding sugar.");
					while (Signals.Sugar > originalSugarLevel - 0.15)
					{
						Thread.Sleep(_appconfig.IngredientAdditionDelayMs);
						Signals.Sugar -= (float)0.01;
					}

					Signals.MakingCoffee = false;
					Dashboard.Sgt.LogAsync($"Ok, we are done.");
				}
			}
			catch (Exception ex)
			{
				return false;
			}

			return true;
		}
	}
}