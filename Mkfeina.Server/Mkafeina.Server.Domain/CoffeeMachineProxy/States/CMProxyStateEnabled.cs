using Mkafeina.Domain.ServerArduinoComm;
using System;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy.States
{
	internal class CMProxyStateEnabled : CMProxyState
	{
		public CMProxyStateEnabled(CMProxy proxy) : base(proxy)
		{
		}

		protected override Action Callback
			=> new Action(() =>
			{
				_proxy.CurrentState = _proxy.DisabledState;
				_proxy.Info.MakingCoffee = false;
				_proxy.Info.Enabled = false;
				LogOnDashAsync($"{_proxy.Info.UniqueName} has not sent news for {WdtLimitTime.Minutes} minutes. I disabled it.");
				CallProxyActionEvent(ProxyEventEnum.MachineDisabledForNotSendingMessagesForTooLong);
			});

		protected override TimeSpan WdtLimitTime => new TimeSpan(0, 10, 0);

		internal override OrderResponse HandleGiveMeAnOrder(OrderRequest request)
		{
			if (!_proxy._waitress.ThereIsOrder())
			{
				_response = _ardResponseFac.InvalidRequest<OrderResponse>(ErrorEnum.MachineAskedForOrderButThereIsNone);
				LogOnDashAsync($"{_proxy.Info.UniqueName} asked for an order but there is none.");
				return (OrderResponse)_response;
			}

			var order = _proxy._waitress.GetOrder();
			_proxy.ProcessingState.OrderUnderProcess = order;
			_proxy.CurrentState = _proxy.ProcessingState;
			_proxy.Info.MakingCoffee = true;
			_response = _ardResponseFac.GiveMeAnOrderOK(order.Reference, _proxy._cookbook[order.RecipeName].ToString());

			LogOnDashAsync($"{_proxy.Info.UniqueName} got an order of {order.RecipeName} (ref: {order.Reference}).");
			CallProxyActionEvent(ProxyEventEnum.SentAnOrder);

			return (OrderResponse)_response;
		}

		internal override ReportResponse HandleSignals(ReportRequest request)
		{
			_proxy.Info.UpdateIngredients(request.Signals);
			if (_proxy.Info.LevelsUnderMinimum)
			{
				_response = _ardResponseFac.ReportOK(CommandEnum.Disable);
				_proxy.Info.Enabled = false;
				_proxy.CurrentState = _proxy.DisabledState;
				LogOnDashAsync($"{_proxy.Info.UniqueName} has its levels under the minimum! I told it to disable.");
				CallProxyActionEvent(ProxyEventEnum.ToldMachineToDisable);
			}
			else
			{
				var thereIsOrder = _proxy._waitress.ThereIsOrder();
				_response = _ardResponseFac.ReportOK(_proxy._waitress.ThereIsOrder() ? CommandEnum.TakeAnOrder : CommandEnum.Void);
			}
			return (ReportResponse)_response;
		}
	}
}