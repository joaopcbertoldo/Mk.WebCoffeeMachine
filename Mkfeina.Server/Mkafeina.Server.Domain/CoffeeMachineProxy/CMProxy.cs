using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.Entities;
using Mkfeina.Server.Domain.CoffeeMachineProxy;
using System;
using System.Collections.Generic;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public class CMProxy
	{
		public static CMProxy CreateCMProxy(string mac, string uniqueName, IngredientsSetup setup)
		{
			var proxy = new CMProxy();
			proxy._info = CMProxyInfo.CreateCMProxyInfo(proxy, mac, uniqueName, setup); ;
			proxy._cookBook = CookBook.CreateCookbook(proxy);
			proxy._waitress = Waitress.CreateWaitress(proxy);
			proxy._ardResponseFac = new ArduinoResponseFactory();
#warning criar todos os estados!!!!!!!!!!!!!!!!!!
#warning preencher estado inicial do proxy!!!!!!!!!!!!!!!!!!
			//proxy._state = ...
			return proxy;
		}

		private CMProxy()
		{
		}

		#region Internal Stuff

		private CookBook _cookBook;

		private Waitress _waitress;

		private ArduinoResponseFactory _ardResponseFac;

		private CMProxyInfo _info;

		#endregion Internal Stuff

		#region States

		internal CMProxyState CurrentState { get; set; }
		//internal CMProxyState CurrentState { get; set; }
		//internal CMProxyState CurrentState { get; set; }
		//internal CMProxyState CurrentState { get; set; }
		//internal CMProxyState CurrentState { get; set; }
		//internal CMProxyState CurrentState { get; set; }
		//internal CMProxyState CurrentState { get; set; }
		//internal CMProxyState CurrentState { get; set; }
		//internal CMProxyState CurrentState { get; set; }

		#endregion States

		public IEnumerable<string> AllRecipesNames { get => _cookBook.AllRecipesNames; }
#warning this needs to be public????????????
		public CMProxyInfo Info { get => _info; }

		public event Action<string, object> ChangeEvent;

		internal void OnChangeEvent(string lineName, object caller) => ChangeEvent?.Invoke(lineName, caller);

		public RegistrationResponse HandleRegistrationAcceptance(RegistrationRequest request) => CurrentState.HandleRegistrationAcceptance(request);

		public RegistrationResponse HandleOffsets(RegistrationRequest request) => CurrentState.HandleOffsets(request);

		public RegistrationResponse HandleUnregistration(RegistrationRequest request) => CurrentState.HandleUnregistration(request);

		public ReportResponse HandleLevels(ReportRequest request) => CurrentState.HandleLevels(request);

		public ReportResponse HandleDisabling(ReportRequest request) => CurrentState.HandleDisabling(request);

		public OrderResponse HandleGiveMeAnOrder(OrderRequest request) => CurrentState.HandleOrderRequest(request);

		public OrderResponse HandleProcessingWillStart(OrderRequest request) => CurrentState.HandleProcessingWillStart(request);

		public OrderResponse HandleOrderReady(OrderRequest request) => CurrentState.HandleOrderReady(request);

		public OrderResponse HandleProblemOcurredDuringProcessing(OrderRequest request) => CurrentState.HandleProblemOcurredDuringProcessing(request);

#warning fazer o dash inscrever no eventoGY
	}
}