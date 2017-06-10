using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.CoffeeMachineProxy.States;
using Mkafeina.Server.Domain.Entities;
using System;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public enum ProxyEventEnum
	{
		Undef = 0,
		ProxyUnregistered = 1,
		IngredientsSetupRedefined = 2,
		ResentAGiveMeAnOrder = 3,
		ToldMachineToDisable = 4,
		MachineDisabledForNotSendingMessagesForTooLong = 5,
		MachineAskedForOrderAgain = 6,
		ToldThatItShouldNotBeProcessing = 7,
		OrderReady = 8,
		ToldMachineToReenable = 9,
		MachineIsDisabled = 10,
		ReceivedReadyFromUnexpectedOrder = 11,
		MachineUnregisteredForNotSendingMessagesForTooLong = 12,
		MachineTakingTooLongToProcess = 13,
		SentAnOrder = 14
	}

	public class CMProxy : IProxyEventObservable
	{
		private const string
			COFFE_MACHINE = "cm";

		public static CMProxy CreateCMProxy(string mac, string uniqueName, IngredientsSetup setup)
		{
			var proxy = new CMProxy();
			proxy._info = CMProxyInfo.CreateCMProxyInfo(proxy, mac, uniqueName, setup); ;
			proxy.DisabledState = new CMProxyStateDisabled(proxy);
			proxy.EnabledState = new CMProxyStateEnabled(proxy);
			proxy.ProcessingState = new CMProxyStateProcessing(proxy);
			var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
			var appconfig = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();
			dash.LogAsync($"New Coffee Machine! Name: {uniqueName}.");
			var config = appconfig.PanelConfigs(COFFE_MACHINE);
			config.Title = uniqueName;
			dash.CreateDynamicPanel(uniqueName, config);
			dash.AddFixedLinesToDynamicPanel(uniqueName, appconfig.PanelFixedLines(COFFE_MACHINE));
			proxy.ChangeEvent += dash.UpdateEventHandlerOfPanel(uniqueName);
			proxy._currentState = proxy.EnabledState;
			proxy.Info.Enabled = true;
			proxy.Info.MakingCoffee = false;
			proxy._cookbook = CookBook.CreateCookbook(proxy);
			proxy._cookbook.GetRecipesFromMainCookbook();
			proxy._waitress = Waitress.CreateWaitress(proxy);
			proxy.Subscribe(proxy._waitress);
			proxy.Subscribe(CMProxyHub.Sgt);

			return proxy;
		}

		private CMProxy()
		{
		}

		#region Internal Stuff

		internal CookBook _cookbook;

		internal Waitress _waitress;

		internal CMProxyInfo _info;

		#endregion Internal Stuff

		public CMProxyInfo Info { get => _info; }

		public event Action<string, object> ChangeEvent;

		internal void OnChangeEvent(string lineName) => ChangeEvent?.Invoke(lineName, Info);

		#region States

#warning carregar estados

		private CMProxyState _currentState;

		internal CMProxyState CurrentState {
			get { return _currentState; }
			set {
				_currentState?.Wdt.Stop();
				_currentState = value;
				_currentState.Wdt.Start();
				OnChangeEvent("state");
			}
		}

		internal CMProxyStateEnabled EnabledState { get; set; }

		internal CMProxyStateProcessing ProcessingState { get; set; }

		internal CMProxyStateDisabled DisabledState { get; set; }

		#endregion States

		#region State calls

		public RegistrationResponse HandleOffsets(RegistrationRequest request) => CurrentState.HandleOffsets(request);

		public RegistrationResponse HandleUnregistration(RegistrationRequest request) => CurrentState.HandleUnregistration(request);

		public ReportResponse HandleSignals(ReportRequest request) => CurrentState.HandleSignals(request);

		public ReportResponse HandleDisabling(ReportRequest request) => CurrentState.HandleDisabling(request);

		public ReportResponse HandleReenable(ReportRequest request) => CurrentState.HandleReenable(request);

		public OrderResponse HandleGiveMeAnOrder(OrderRequest request) => CurrentState.HandleGiveMeAnOrder(request);

		public OrderResponse HandleReady(OrderRequest request) => CurrentState.HandleReady(request);

		public OrderResponse HandleCancelOrder(OrderRequest request) => CurrentState.HandleCancelOrder(request);

		#endregion State calls

		public void Subscribe(IProxyEventObserver observer)
		{
			EnabledState.ProxyActionEvent += observer.Notify;
			ProcessingState.ProxyActionEvent += observer.Notify;
			DisabledState.ProxyActionEvent += observer.Notify;
		}
	}
}