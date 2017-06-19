using Microsoft.Practices.Unity;
using Mkafeina.Domain;
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
		SentAnOrder = 14,
		MachineDisabledWithoutWarning = 15
	}

	public class CMProxy : IProxyEventObservable
	{
		private const string
			COFFE_MACHINE = "cm"
			;

		public const string
			CMPROXY_STATE = "state",
			CMPROXY_RECIPES = "recipes"
			;

		#region Internal Stuff

		internal ProxyCookBook Cookbook { get; private set; }

		internal Waitress Waitress { get; private set; }

		public CMProxyInfo Info { get; private set; }

		public event Action<string, object> ChangeEvent;

		internal void OnChangeEvent(string lineName) => ChangeEvent?.Invoke(lineName, this);

		private CMProxyState _currentState;

		internal bool _disableFlag;
		internal bool _unregisterFlag;

		internal CMProxyState CurrentState {
			get { return _currentState; }
			set {
				_currentState?.Wdt.Stop();
				_currentState = value;
				_currentState.Wdt.Start();
				OnChangeEvent(CMPROXY_STATE);
			}
		}

		#region States

		internal CMProxyStateEnabled EnabledState { get; private set; }

		internal CMProxyStateProcessing ProcessingState { get; private set; }

		internal CMProxyStateDisabled DisabledState { get; private set; }

		#endregion States

		#endregion Internal Stuff

		internal CMProxy(RegistrationRequest request)
		{
			var appconfig = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();

			Info = new CMProxyInfo(request, this);

			var config = appconfig.PanelConfigs(COFFE_MACHINE);
			config.Title = request.un;
			Dashboard.Sgt.CreateDynamicPanel(request.un, config);
			Dashboard.Sgt.AddFixedLinesToDynamicPanel(request.un, appconfig.PanelFixedLines(COFFE_MACHINE));
			ChangeEvent += Dashboard.Sgt.UpdateEventHandlerOfPanel(request.un);

			DisabledState = new CMProxyStateDisabled(this);
			EnabledState = new CMProxyStateEnabled(this);
			ProcessingState = new CMProxyStateProcessing(this);
			CurrentState = EnabledState;
			_disableFlag = false;

			Info.Enabled = true;
			Info.MakingCoffee = false;

			Cookbook = new ProxyCookBook(this);
			Cookbook.GetRecipesFromMainCookbook();
			Waitress = new Waitress(this);

			Subscribe(Waitress);
			Subscribe(CMProxyHub.Sgt);
		}


		public void Subscribe(IProxyEventObserver observer)
		{
			EnabledState.ProxyActionEvent += observer.Notify;
			ProcessingState.ProxyActionEvent += observer.Notify;
			DisabledState.ProxyActionEvent += observer.Notify;
		}

		public void SetDisableFlag()
		{
			_disableFlag = true;
		}


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
	}
}