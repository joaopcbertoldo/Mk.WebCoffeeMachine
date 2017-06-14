using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.Entities;
using System;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy.States
{
	internal class CMProxyStateProcessing : CMProxyState
	{
		internal Order OrderUnderProcess { get; set; }

		protected override Action Callback
			=> new Action(() =>
			{
				_proxy.CurrentState = _proxy.DisabledState;
				_proxy.Info.Enabled = false;
				LogOnDashAsync($"{_proxy.Info.UniqueName} is taking too long to process {OrderUnderProcess.RecipeName} (ref: {OrderUnderProcess.Reference}). I disabled it.");
				CallProxyActionEvent(ProxyEventEnum.MachineTakingTooLongToProcess);
			});

		protected override TimeSpan WdtLimitTime => new TimeSpan(0, 5, 0);

		public CMProxyStateProcessing(CMProxy proxy) : base(proxy)
		{
		}

		internal override RegistrationResponse HandleOffsets(RegistrationRequest request)
		{
			_response = _ardResponseFac.InvalidRequest<RegistrationResponse>(ErrorEnum.ShouldNotSentOffsets, CommandEnum.TakeAnOrder);
			LogOnDashAsync($"{_proxy.Info.UniqueName} has sent an 'offsets' but it should be processing.");
			CallProxyActionEvent(ProxyEventEnum.ResentAGiveMeAnOrder);
			return (RegistrationResponse)_response;
		}

		internal override OrderResponse HandleGiveMeAnOrder(OrderRequest request)
		{
			// OLD VERSION
			//_response = _ardResponseFac.GiveMeAnOrderAgain(OrderUnderProcess.Reference, _proxy.Cookbook[OrderUnderProcess.RecipeName].ToString());
			_response = _ardResponseFac.GiveMeAnOrderAgain(OrderUnderProcess.Reference, _proxy.Cookbook[OrderUnderProcess.RecipeName].ToRecipeObj());
			LogOnDashAsync($"{_proxy.Info.UniqueName} asked for an order but it's already processing a {OrderUnderProcess.RecipeName} (ref: {OrderUnderProcess.Reference}).");
			CallProxyActionEvent(ProxyEventEnum.MachineAskedForOrderAgain);
			return (OrderResponse)_response;
		}

		internal override OrderResponse HandleReady(OrderRequest request)
		{
			if (request.OrderReference != OrderUnderProcess.Reference)
			{
				_response = _ardResponseFac.InvalidRequest<OrderResponse>(ErrorEnum.WrongOrderReference, CommandEnum.Disable);
				CallProxyActionEvent(ProxyEventEnum.ReceivedReadyFromUnexpectedOrder);
				LogOnDashAsync($"{_proxy.Info.UniqueName} called ready for order {request.OrderReference} but should be processing {OrderUnderProcess.Reference}.");
				CallProxyActionEvent(ProxyEventEnum.ToldMachineToDisable);
				return (OrderResponse)_response;
			}
			_response = _ardResponseFac.ReadyOK();
			OrderUnderProcess = null;
			CallProxyActionEvent(ProxyEventEnum.OrderReady);
			_proxy.CurrentState = _proxy.EnabledState;
			_proxy.Info.MakingCoffee = false;
			return (OrderResponse)_response;
		}

		internal override ReportResponse HandleSignals(ReportRequest request)
		{
			_response = _ardResponseFac.InvalidRequest<ReportResponse>(ErrorEnum.ShouldNotSendSignals, CommandEnum.Process);
			LogOnDashAsync($"{_proxy.Info.UniqueName} sent signals but should be processing a {OrderUnderProcess.RecipeName} (ref: {OrderUnderProcess.Reference}).");
			return (ReportResponse)_response;
		}
	}
}