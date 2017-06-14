using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Server.Domain.CustomerApi;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public enum WaitressStatusEnum
	{
		Undef = 0,
		NoOrder,
		OrdersWaiting,
		OrderWasTaken,
		Processing
	}

	public class Waitress : IProxyEventObserver
	{
		#region Static Stuff

		private static Dictionary<string, Waitress> __waitresses = new Dictionary<string, Waitress>();

		internal static void DeleteWaitress(string mac)
		{
			lock (__waitresses)
				__waitresses.Remove(mac);
		}

		public static Waitress GetWaitress(string mac)
		{
			lock (__waitresses)
			{
				return __waitresses.ContainsKey(mac) ? __waitresses[mac] : null;
			}
		}

		#endregion Static Stuff

		#region Internal Stuff

		private CustomerResponseFactory _custResponseFac;
		private CMProxy _owner;
		private Queue<Order> _queue;
		private Order _orderUnderProcessing;
		private int _queueCapacity;
		private int _minimumSecondsBetweenOrders;
		private EmailSender _emailSender;
		private DateTime _lastFinishedOrderTime;
		private WaitressStatusEnum _status;

		#endregion Internal Stuff

		internal Waitress(CMProxy owner)
		{
			var appconfig = (AppConfig)AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();

			_owner = owner;
			_status = WaitressStatusEnum.NoOrder;
			_minimumSecondsBetweenOrders = appconfig.MinimumSecondsBetweenOrders;
			_queueCapacity = appconfig.WaitressCapacity;
			_lastFinishedOrderTime = DateTime.UtcNow.Subtract(new TimeSpan(0, 1, 0));

			_emailSender = new EmailSender(_owner);
			_custResponseFac = new CustomerResponseFactory();
			_queue = new Queue<Order>();

			lock (__waitresses)
				__waitresses.Add(owner.Info.Mac, this);
		}

		internal bool ThereIsOrder() => _queue.Count > 0 && (int)(DateTime.UtcNow - _lastFinishedOrderTime).TotalSeconds >= _minimumSecondsBetweenOrders;

		internal Order GetOrder()
		{
			lock (_queue)
			{
				_orderUnderProcessing = _queue.Dequeue();
				_orderUnderProcessing.Status = OrderStatusEnum.Taken;
				_orderUnderProcessing.TakenTime = DateTime.UtcNow;
				if (_queue.Count > 0)
				{
					_emailSender.SendMailOrderTakenAsync(_orderUnderProcessing);
					for (var i = 0; _queue.Count > i; i++)
						_emailSender.SendMailQueuePositionHasChangedAsync(_queue.ElementAt(i), i + 1);
				}
				Dashboard.Sgt.LogAsync($"Order {_orderUnderProcessing.Reference} ({_orderUnderProcessing.RecipeName}) has been taken by {_owner.Info.UniqueName}.");
				return _orderUnderProcessing;
			}
		}

		internal void CancelAllOrders()
		{
			Task.Factory.StartNew(() =>
			{
				if (_orderUnderProcessing != null)
				{
					_orderUnderProcessing.Status = OrderStatusEnum.Canceled;
					_orderUnderProcessing.ReadyOrCanceledTime = DateTime.UtcNow;
					_emailSender.SendMailOrderCanceledAsync(_orderUnderProcessing);
				}

				foreach (var order in _queue)
				{
					_orderUnderProcessing.Status = OrderStatusEnum.Canceled;
					_orderUnderProcessing.ReadyOrCanceledTime = DateTime.UtcNow;
					_emailSender.SendMailOrderCanceledAsync(order);
				}

				Dashboard.Sgt.LogAsync($"All orders of {_owner.Info.UniqueName} have been canceled.");
				Thread.Sleep(15000);
				while (_queue.Count > 0)
					_queue.Dequeue();
			});
		}

		public void Notify(ProxyEventEnum action)
		{
#warning mover os logs para ca ou para o hub (TIRAR DOS STATES)
			switch (action)
			{
				case ProxyEventEnum.OrderReady:
					Dashboard.Sgt.LogAsync($"{_owner.Info.UniqueName} finished an order of {_orderUnderProcessing.RecipeName} (ref: {_orderUnderProcessing.Reference}).");
					_emailSender.SendMailOrderReadyAsync(_orderUnderProcessing);
					_orderUnderProcessing = null;
					return;

				default:
					return;
			}
		}

		public CustomerOrderResponse HandleCustomerOrder(CustomerOrderRequest request)
		{
			if (!_owner.Info.Enabled)
				return _custResponseFac.CMCurrentlyDisabled();

			if (!_owner.Info.HasRecipe(request.RecipeName))
				return _custResponseFac.RecipeNotAvailable();

			lock (_queue)
			{
				if (_queue.Count >= _queueCapacity)
					return _custResponseFac.FullQueue();

				var newOrder = new Order()
				{
					CustomerEmail = request.CustomerEmail,
					RecipeName = request.RecipeName,
					Reference = (new Random((int)DateTime.Now.ToBinary()).Next()).ToString("X"),
					Status = OrderStatusEnum.InQueue,
					CreationTime = DateTime.UtcNow
				};

				_queue.Enqueue(newOrder);
				var positionInQueue = _queue.Count;

				Dashboard.Sgt.LogAsync($"{_owner.Info.UniqueName}'s waitress has got a new order! Recipe: {newOrder.RecipeName}, ref: {newOrder.Reference}.");
				return _custResponseFac.OrderReceived(positionInQueue, request.CustomerEmail);
			}
		}
	}
}