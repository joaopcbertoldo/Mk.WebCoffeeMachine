using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.Dashboard;
using Mkafeina.Domain.ServerArduinoComm;
using System;
using System.Threading.Tasks;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy.States
{
	internal abstract class CMProxyState
	{
		#region Internal Stuff

		protected CMProxy _proxy;

		protected ArduinoResponseFactory _ardResponseFac;

		protected ArduinoResponse _response;

		internal WatchDogTimer Wdt { get; private set; }

		protected abstract Action Callback { get; }

		protected abstract TimeSpan WdtLimitTime { get; }

		#endregion Internal Stuff

		internal CMProxyState(CMProxy proxy)
		{
			_proxy = proxy;
			_ardResponseFac = new ArduinoResponseFactory();
			Wdt = new WatchDogTimer(Callback);
			Wdt.LimitTime = WdtLimitTime;
		}

		internal event Action<ProxyEventEnum> ProxyActionEvent;

		protected void CallProxyActionEvent(ProxyEventEnum action) => ProxyActionEvent?.Invoke(action);

		internal virtual RegistrationResponse HandleOffsets(RegistrationRequest request)
		{
			_response = _ardResponseFac.RegistrationOK();

			if (request.stp == null)
				return _ardResponseFac.InvalidRequest<RegistrationResponse>(ErrorEnum.MissingIngredientsSetup);
			_proxy.Info.SetupAvaiabilityAndOffsets(request.stp);
			LogOnDashAsync($"{_proxy.Info.UniqueName} has redefined the ingredients' setup.");
			CallProxyActionEvent(ProxyEventEnum.IngredientsSetupRedefined);
			return (RegistrationResponse)_response;
		}

		internal virtual RegistrationResponse HandleUnregistration(RegistrationRequest request)
		{
			_response = _ardResponseFac.RegistrationOK(CommandEnum.Unregister);
		
			var name = _proxy.Info.UniqueName;
			var mac = _proxy.Info.Mac;
			CMProxyHub.Sgt.Unregister(mac);

			var task = Task.Factory.StartNew(() =>
			{
				var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
				dash.LogAsync($"{name} ({mac}) has been unregistered :(.");
			});

			CallProxyActionEvent(ProxyEventEnum.ProxyUnregistered);

			return (RegistrationResponse)_response;
		}

		internal abstract ReportResponse HandleSignals(ReportRequest request);

		internal virtual ReportResponse HandleDisabling(ReportRequest request)
		{
			_response = _ardResponseFac.ReportOK(CommandEnum.Disable);
			_proxy.CurrentState = _proxy.DisabledState;
			_proxy.Info.Enabled = false;
			_proxy.Info.MakingCoffee = false;
			LogOnDashAsync($"{_proxy.Info.UniqueName} is disabled.");
			CallProxyActionEvent(ProxyEventEnum.MachineIsDisabled);
			return (ReportResponse)_response;
		}

		internal virtual ReportResponse HandleReenable(ReportRequest request)
		{
			_response = _ardResponseFac.InvalidRequest<ReportResponse>(ErrorEnum.ShouldBeAlreadyEnabled, CommandEnum.Enable);
			_proxy.Info.Enabled = true;
			LogOnDashAsync($"{_proxy.Info.UniqueName} asked to reenable but should already be enabled.");
			return (ReportResponse)_response;
		}

		internal abstract OrderResponse HandleGiveMeAnOrder(OrderRequest request);

		internal virtual OrderResponse HandleReady(OrderRequest request)
		{
			_response = _ardResponseFac.InvalidRequest<OrderResponse>(ErrorEnum.ShouldNotBeProcessing);
			_proxy.Info.Enabled = true;
			_proxy.Info.MakingCoffee = false;
			CallProxyActionEvent(ProxyEventEnum.ToldThatItShouldNotBeProcessing);
			LogOnDashAsync($"{_proxy.Info.UniqueName} called ready for order {request.oref} but should not be processing.");
			return (OrderResponse)_response;
		}

		internal virtual OrderResponse HandleCancelOrder(OrderRequest request)
		{
			_response = _ardResponseFac.CancelOrderResponse(this.GetType() == typeof(CMProxyStateProcessing) ? ErrorEnum.Void : ErrorEnum.ShouldNotBeProcessing);
			_proxy.Waitress.CancelAllOrders();
			_proxy.Info.Enabled = false;
			_proxy.Info.MakingCoffee = false;
			LogOnDashAsync($"{_proxy.Info.UniqueName} called CANCEL ORDER, I told it to disable.");
			CallProxyActionEvent(ProxyEventEnum.ToldMachineToDisable);
			return (OrderResponse)_response;
		}

		protected void LogOnDashAsync(string msg)
		{
			var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
			dash.LogAsync(msg);
		}
	}
}