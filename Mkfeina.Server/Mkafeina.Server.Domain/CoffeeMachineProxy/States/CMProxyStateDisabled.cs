using Mkafeina.Domain.ServerArduinoComm;
using System;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy.States
{
	internal class CMProxyStateDisabled : CMProxyState
	{
		public CMProxyStateDisabled(CMProxy proxy) : base(proxy)
		{
		}

		protected override Action Callback
			=> new Action(() =>
			{
				LogOnDashAsync($"{_proxy.Info.UniqueName} has not sent news for {WdtLimitTime.Minutes} minutes. I unregistered it.");
				CallProxyActionEvent(ProxyEventEnum.MachineUnregisteredForNotSendingMessagesForTooLong);
				CMProxyHub.Sgt.Unregister(_proxy.Info.Mac);
			});

		protected override TimeSpan WdtLimitTime => new TimeSpan(0, 10, 0);

		internal override OrderResponse HandleGiveMeAnOrder(OrderRequest request)
		{
			_response = _ardResponseFac.InvalidRequest<OrderResponse>(ErrorEnum.MachineDisabledCannotTakeOrders, CommandEnum.Disable);
			LogOnDashAsync($"{_proxy.Info.UniqueName} has asked for an order but it is disabled.");
			CallProxyActionEvent(ProxyEventEnum.ToldMachineToDisable);
			return (OrderResponse)_response;
		}

		internal override ReportResponse HandleReenable(ReportRequest request)
		{
			_response = _ardResponseFac.ReportOK(CommandEnum.Enable);
			_proxy.CurrentState = _proxy.EnabledState;
			_proxy.Info.Enabled = true;
			LogOnDashAsync($"{_proxy.Info.UniqueName} has been reenabled!!!.");
			CallProxyActionEvent(ProxyEventEnum.ToldMachineToReenable);
			return (ReportResponse)_response;
		}

		internal override ReportResponse HandleSignals(ReportRequest request)
		{
				_response = _ardResponseFac.ReportOK();
				_proxy.Info.UpdateIngredients(request.sig);
			return (ReportResponse)_response;
		}
	}
}