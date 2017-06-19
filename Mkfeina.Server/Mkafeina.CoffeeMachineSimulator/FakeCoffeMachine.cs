using Microsoft.Practices.Unity;
using Mkafeina.CoffeeMachineSimulator;
using Mkafeina.Domain;
using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.ServerArduinoComm;
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

		internal bool IsOn { get => _onOffButton; }

		internal CMSignals Signals { get; set; }

		internal string _orderRef;

		// OLD VERSION
		//internal Recipe _recipe = null;
		internal RecipeObj _recipe = null;


		private Task _mainTask;

		internal bool _reenableFlag;
		private AppConfig _appconfig;
		internal bool _disableFlag;

		#region Singleton Stuff

		private static object _singletonSync = new object();

		private static FakeCoffeMachine __sgt;
		internal bool _insertProblemFlag;
		internal bool _sendOffsetsFlag;
		internal CommandEnum commandReg;
		private bool _onOffButton;

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
			_mainTask = CreateMainTask();
		}

		#endregion Singleton Stuff

		public void StartMaintask()
		{
			_onOffButton = true;
			if (_mainTask != null && _mainTask.Status != TaskStatus.Running)
			{
				_mainTask.Start();
				Dashboard.Sgt.LogAsync("Main task started.");
			}
		}

		public void TurnOn()
		{
			_onOffButton = true;
			Dashboard.Sgt.LogAsync("ON");
		}

		public void TurnOff()
		{
			_onOffButton = false;
			Dashboard.Sgt.LogAsync("OFF");
		}

		private Task CreateMainTask()
			=> new Task(() =>
			   {
				   bool ack = false;
				   commandReg = _serverCaller.RegisterNoMatterWhat();
				   while (true)
				   {
					   if (!_onOffButton)
					   {
						   ack = false;
						   for (var i = 0; i < 20 && !ack; i++)
						   {
							   ack = _serverCaller.TryToUnregister();
						   }
						   Signals.Registered = false;
						   Signals.Enabled = false;
						   while (!_onOffButton) { }
						   commandReg = _serverCaller.RegisterNoMatterWhat();
						   continue;
					   }

					   if (_disableFlag)
					   {
						   _disableFlag = false;
						   Signals.Enabled = false;
						   ack = false;
						   for (var i = 0; i < 20 && !ack; i++)
						   {
							   ack = _serverCaller.TryToDisable(out commandReg);
						   }
						   continue;
					   }

					   // not available on the prototipe
					   if (_sendOffsetsFlag)
					   {
						   _sendOffsetsFlag = false;
						   ack = false;
						   for (var i = 0; i < 20 && !ack; i++)
						   {
							   ack = _serverCaller.TryToSendNewOffsets(out commandReg);
						   }
						   continue;
					   }

					   if (Signals.Enabled)
					   {
						   switch (commandReg)
						   {
							   case CommandEnum.Void:
								   _serverCaller.TryToReportSignals(out commandReg);
								   break;

							   case CommandEnum.Disable:
								   Signals.Enabled = false;
								   commandReg = CommandEnum.Void;
								   break;

							   case CommandEnum.Register:
								   commandReg = _serverCaller.RegisterNoMatterWhat();
								   break;

							   case CommandEnum.Process:
								   if (_orderRef == null || _recipe == null)
								   {
									   commandReg = CommandEnum.TakeAnOrder;
								   }
								   else
								   {
									   ack = false;
									   ack = MakeCoffee(_recipe);

									   if (ack)
									   {
										   ack = false;
										   for (var i = 0; i < 20 && !ack; i++)
										   {
											   ack = _serverCaller.TryReady(out commandReg);
										   }

										   if (!ack)
											   commandReg = CommandEnum.Void;
									   }
									   else
									   {
										   ack = false;
										   for (var i = 0; i < 20 && !ack; i++)
										   {
											   ack = _serverCaller.TryCancelOrders(out commandReg);
										   }

										   if (!ack)
											   commandReg = CommandEnum.Disable;
									   }

									   _orderRef = null;
									   _recipe = null;
								   }
								   break;

							   case CommandEnum.TakeAnOrder:
								   ack = false;
								   for (var i = 0; i < 20 && !ack; i++)
								   {
									   ack = _serverCaller.TryToTakeOrder(out commandReg, out _orderRef, out _recipe);
								   }
								   break;

							   default:
								   commandReg = CommandEnum.Void;
								   continue;
						   }
					   }
					   else
					   {
						   if (_reenableFlag)
						   {
							   _reenableFlag = false;
							   commandReg = _serverCaller.ReenableNoMatterWhat();
						   }
						   else
						   {
							   _serverCaller.TryToReportSignals(out commandReg);
						   }
					   }

					   Thread.Sleep(2500);
				   }
			   });

		// OLD
		//private Task CreateMainTask()
		//	=> new Task(() =>
		//	   {
		//		   while (true)
		//		   {
		//			   if (_mainTaksCancelTokenSource.Token.IsCancellationRequested)
		//				   return;

		//			   _serverCaller.TryToReportSignals(out _command);

		//			   if (_disableFlag)
		//			   {
		//				   _disableFlag = false;
		//				   Signals.Enabled = false;
		//				   for (var i = 0; i < 20; i++) { if (_serverCaller.TryToDisable(out _command)) break; }
		//				   continue;
		//			   }

		//			   if (_sendOffsetsFlag)
		//			   {
		//				   _sendOffsetsFlag = false;
		//				   for (var i = 0; i < 20; i++) { if (_serverCaller.TryToSendNewOffsets(out _command)) break; }
		//				   continue;
		//			   }

		//			   if (Signals.Enabled)
		//			   {
		//				   switch (_command)
		//				   {
		//					   case CommandEnum.Void:
		//						   break;

		//					   case CommandEnum.Disable:
		//						   Signals.Enabled = false;
		//						   continue;

		//					   case CommandEnum.Unregister:
		//						   Task.Factory.StartNew(() => TurnOff());
		//						   break;

		//					   default:
		//						   break;
		//				   }

		//				   if (_command == CommandEnum.TakeAnOrder || _command == CommandEnum.Process)
		//				   {
		//					   for (var i = 0; i < 20; i++) { if (_serverCaller.TryToTakeOrder(out _command, out _orderRef, out _recipe)) break; }
		//					   if (_command == CommandEnum.Process)
		//					   {
		//						   if (_insertProblemFlag)
		//						   {
		//							   _insertProblemFlag = false;
		//							   for (var i = 0; i < 20; i++) { if (_serverCaller.TryCancelOrders(out _command)) break; }
		//							   Signals.Enabled = false;
		//							   continue;
		//						   }

		//						   // OLD VERSION
		//						   //var ack = MakeCoffee(Recipe.Parse(_recipe));
		//						   var ack = MakeCoffee(_recipe);
		//						   if (ack)
		//						   {
		//							   for (var i = 0; i < 20; i++) { if (_serverCaller.TryReady(out _command)) break; }
		//						   }
		//						   else
		//						   {
		//							   for (var i = 0; i < 20; i++) { if (_serverCaller.TryCancelOrders(out _command)) break; }
		//						   }
		//					   }
		//				   }
		//			   }
		//			   else if (_reenableFlag)
		//			   {
		//				   while (!_serverCaller.TryToReenable(out _command))
		//				   { }
		//				   _reenableFlag = false;
		//			   }
		//			   Thread.Sleep(2500);
		//		   }
		//	   }, _mainTaksCancelTokenSource.Token);

		// OLD VERSION
		//public bool MakeCoffee(Recipe recipe)

		public bool MakeCoffee(RecipeObj recipe)
		{
			try
			{
				lock (this)
				{
					Signals.MakingCoffee = true;

					// OLD VERSION
					//var coffeeMeasures = recipe["Coffee"] * 0.15;
					var coffeeMeasures = recipe.c * 0.15;
					var originalCoffeeLevel = Signals.Coffee;

					// OLD VERSION
					//var waterMl = (Signals.CoffeeMax - Signals.CoffeeMin) * recipe["Water"] / 1000;
					var waterMl = (Signals.CoffeeMax - Signals.CoffeeMin) * recipe.w / 1000;
					var originalWaterLevel = Signals.Water;

					// OLD VERSION
					//var sugarMeasures = recipe["Sugar"] * 0.15;
					var sugarMeasures = recipe.s * 0.15;
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
#warning jogar exception no db
				return false;
			}

			return true;
		}
	}
}