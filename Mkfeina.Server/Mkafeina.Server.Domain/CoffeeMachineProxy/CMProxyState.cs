using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.CoffeeMachineProxy;
using System;

namespace Mkfeina.Server.Domain.CoffeeMachineProxy
{
	//{
	//		lock (_proxies)
	//		{
	//			if (!_proxies.ContainsKey(request.Mac))
	//				return _ardResponseFac.RegistrationInvalidRequest();

	//			var name = _proxies[request.Mac].Info.UniqueName;
	//_proxies.Remove(request.Mac);

	//			var task = Task.Factory.StartNew(() =>
	//			{
	//				var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
	//				dash.LogAsync($"{name} have been unregistered :(.");
	//			});

	//			return _ardResponseFac.RegistrationOK();
	//		}
	//	}
	//}

	//		{
	//			lock (_proxies)
	//			{
	//				if (!_proxies.ContainsKey(request.Mac))
	//					return _ardResponseFac.RegistrationInvalidRequest();
	//			}

	//var proxy = _proxies[request.Mac];

	//			lock (proxy)
	//			{
	//				proxy.Info.SetupAvaiabilityAndOffsets(request.IngredientsSetup);

	//				var task = Task.Factory.StartNew(() =>
	//				{
	//					var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
	//					dash.LogAsync($"{proxy.Info.UniqueName}'s offsets have been reseted.");
	//				});

	//				return _ardResponseFac.RegistrationOK();
	//			}
	//		}

	//	{
	//			lock (_proxies)
	//			{
	//				if (!_proxies.ContainsKey(request.Mac))
	//					return _ardResponseFac.RegistrationInvalidRequest();
	//			}

	//var proxy = _proxies[request.Mac];

	//			lock (proxy)
	//			{
	//				if (proxy.Info.RegistrationIsAccepted)
	//					return _ardResponseFac.RegistrationAcceptanceButIsAlreadyAccepted();
	//				else
	//				{
	//					proxy.Info.RegistrationIsAccepted = true;

	//					var task = Task.Factory.StartNew(() =>
	//					{
	//						var dash = AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>();
	//						dash.LogAsync($"{proxy.Info.UniqueName} registration accepted.");
	//					});

	//					return _ardResponseFac.RegistrationOK();
	//				}
	//			}
	//		}

	//{
	//		switch (request.ReportMessage)
	//		{
	//			case ReportMessageEnum.Levels:
	//				_info.Update(request);

	//				if (_info.LevelsUnderMinimum())
	//					return _ardResponseFac.ReportOKDisable();

	//				if (_waitress.ThereIsOrder())
	//					return _ardResponseFac.ReportOKGetOrder();
	//				else
	//					return _ardResponseFac.ReportOKDoNothing();

	//			case (int) ReportMessageEnum.DisablingCoffeeMachine:
	//				_info.Enabled = false;
	//				return _ardResponseFac.ReportOKConfirmDisabling();

	//			default:
	//				return _ardResponseFac.ReportInvalidRequest();
	//		}
	//	}

	//		{
	//			switch (request.OrderMessage)
	//			{
	//				case (int) OrderMessageEnum.GiveMeAnOrder:
	//					{
	//		var order = _waitress.GetOrder();
	//		if (order != null)
	//			return _ardResponseFac.OrderOKGiveMeAnOrder(order.Reference, _cookBook[order.RecipeName].ToString());
	//		else
	//			return _ardResponseFac.OrderInvalidRequest();
	//	}

	//				case (int) OrderMessageEnum.ProcessingWillStart:
	//					{
	//		var ack = _waitress.NotifyThatOrderIsBeingProcessed();
	//		if (ack)
	//			return _ardResponseFac.OrderOKProcessingWilStart();
	//		else
	//			return _ardResponseFac.OrderInvalidRequest();
	//	}

	//				case (int) OrderMessageEnum.OrderReady:
	//					{
	//		var ack = _waitress.NotifyThatOrderIsReady();
	//		if (ack)
	//			return _ardResponseFac.OrderOKReady();
	//		else
	//			return _ardResponseFac.OrderInvalidRequest();
	//	}

	//				case (int) OrderMessageEnum.ProblemOcurredDuringProcessing:
	//#warning implementar pra quando da erro
	//					return _ardResponseFac.OrderOKProblemOccurredDuringProcessing();

	//				default:
	//					return _ardResponseFac.OrderInvalidRequest();
	//			}
	//		}

	internal abstract class CMProxyState
	{
		protected CMProxy _proxy;

		protected ArduinoResponseFactory _ardResponseFac;

		internal CMProxyState(CMProxy proxy)
		{
			_proxy = proxy;
			_ardResponseFac = new ArduinoResponseFactory();
		}

		internal RegistrationResponse HandleRegistrationAcceptance(RegistrationRequest request)
		{
			throw new NotImplementedException();
		}

		internal RegistrationResponse HandleOffsets(RegistrationRequest request)
		{
			throw new NotImplementedException();
		}

		internal RegistrationResponse HandleUnregistration(RegistrationRequest request)
		{
			throw new NotImplementedException();
		}

		internal ReportResponse HandleLevels(ReportRequest request)
		{
			throw new NotImplementedException();
		}

		internal ReportResponse HandleDisabling(ReportRequest request)
		{
			throw new NotImplementedException();
		}

		internal OrderResponse HandleOrderRequest(OrderRequest request)
		{
			throw new NotImplementedException();
		}

		internal OrderResponse HandleProcessingWillStart(OrderRequest request)
		{
			throw new NotImplementedException();
		}

		internal OrderResponse HandleOrderReady(OrderRequest request)
		{
			throw new NotImplementedException();
		}

		internal OrderResponse HandleProblemOcurredDuringProcessing(OrderRequest request)
		{
			throw new NotImplementedException();
		}
	}
}