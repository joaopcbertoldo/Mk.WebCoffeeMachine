using Mkafeina.Domain.ArduinoApi;
using Mkafeina.Domain.ServerArduinoComm;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public class CMProxy
	{
		#region Internal Stuff

		private CookBook _cookBook;

		private Waitress _waitress;

		private ArduinoResponseFactory _ardResponseFac;

		private CMProxyState _state;

		#endregion Internal Stuff

		public CMProxyState State { get => _state; }

		public event Action<string, object> ChangeEvent;

		internal void OnStateChangeEvent(string lineName, object caller) => ChangeEvent?.Invoke(lineName, caller);

		public static CMProxy CreateCMProxy(string mac, string uniqueName, IngredientsSetup setup)
		{
			var proxy = new CMProxy();
			proxy._state = CMProxyState.CreateCMProxyState(proxy, mac, uniqueName, setup); ;
			proxy._cookBook = CookBook.CreateCookbook(proxy);
			proxy._waitress = Waitress.CreateWaitress(proxy);
			proxy._ardResponseFac = new ArduinoResponseFactory();
			return proxy;
		}

		private CMProxy()
		{
		}

		public ReportResponse HandleReportRequest(ReportRequest request)
		{
			switch (request.ReportMessage)
			{
				case (int)ReportMessageEnum.Levels:
					_state.Update(request);

					if (_state.LevelsUnderMinimum())
						return _ardResponseFac.ReportOKDisable();

					if (_waitress.ThereIsOrder())
						return _ardResponseFac.ReportOKGetOrder();
					else
						return _ardResponseFac.ReportOKDoNothing();

				case (int)ReportMessageEnum.DisablingCoffeeMachine:
					_state.Enabled = false;
					return _ardResponseFac.ReportOKConfirmDisabling();

				default:
					return _ardResponseFac.ReportInvalidRequest();
			}
		}

		public OrderResponse HandleOrderRequest(OrderRequest request)
		{
			switch (request.OrderMessage)
			{
				case (int)OrderMessageEnum.GiveMeAnOrder:
					{
						var order = _waitress.GetOrder();
						if (order != null)
							return _ardResponseFac.OrderOKGiveMeAnOrder(order.Reference, _cookBook[order.RecipeName].ToString());
						else
							return _ardResponseFac.OrderInvalidRequest();
					}

				case (int)OrderMessageEnum.ProcessingWillStart:
					{
						var ack = _waitress.NotifyThatOrderIsBeingProcessed();
						if (ack)
							return _ardResponseFac.OrderOKProcessingWilStart();
						else
							return _ardResponseFac.OrderInvalidRequest();
					}

				case (int)OrderMessageEnum.OrderReady:
					{
						var ack = _waitress.NotifyThatOrderIsReady();
						if (ack)
							return _ardResponseFac.OrderOKReady();
						else
							return _ardResponseFac.OrderInvalidRequest();
					}

				case (int)OrderMessageEnum.ProblemOcurredDuringProcessing:
#warning implementar pra quando da erro 
					return _ardResponseFac.OrderOKProblemOccurredDuringProcessing();

				default:
					return _ardResponseFac.OrderInvalidRequest();
			}
		}

#warning fazer o dash inscrever no eventoGY

		public IEnumerable<string> AllRecipesNames { get => _cookBook.AllRecipesNames; }
	}
}