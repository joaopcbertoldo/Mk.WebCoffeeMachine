using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Server.Domain.CustomerApi;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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

	public class Waitress
	{
		#region Static Stuff

		private static string DefaultSignature(string uniqueName)
			=> $"MKafeína - {uniqueName}\r\n" +
			   "mkafeina@gmail.com\r\n" +
			   "São Carlos, SP, Brazil\r\n" +
			   "Avenida Trabalhador São-Carlense, 400, Pq Arnold Schimidt\r\n" +
			   "Prédio da Engenharia Mecatrônica, USP\r\n" +
			   "\r\n" +
			   "© 2017 - MKafeína - Engenharia Mecatrônica EESC USP";

		private static Dictionary<string, Waitress> __waitresses = new Dictionary<string, Waitress>();

		public static Waitress GetWaitress(string mac)
		{
			lock (__waitresses)
			{
				return __waitresses.ContainsKey(mac) ? __waitresses[mac] : null;
			}
		}

		internal static Waitress CreateWaitress(CMProxy cmProxy)
		{
			var appconfig = (AppConfig)AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();
			var waitress = new Waitress()
			{
				_boss = cmProxy,
				_custResponseFac = new CustomerResponseFactory(),
				_queue = new Queue<Order>(),
				_queueCapacity = appconfig.WaitressCapacity,
				_minimumSecondsBetweenOrders = appconfig.MinimumSecondsBetweenOrders,
				_emailSender = EmailSender.CreateEmailSender(DefaultSignature(cmProxy.Info.UniqueName)),
				_status = WaitressStatusEnum.NoOrder
			};

			lock (__waitresses)
				__waitresses.Add(cmProxy.Info.Mac, waitress);

			return waitress;
		}

		#endregion Static Stuff

		#region Internal Stuff

		private CustomerResponseFactory _custResponseFac;
		private CMProxy _boss;
		private Queue<Order> _queue;
		private Order _orderDequeued;
		private int _queueCapacity;
		private int _minimumSecondsBetweenOrders;
		private EmailSender _emailSender;
		private DateTime _lastFinishedOrderTime;

		#endregion Internal Stuff

		private Waitress()
		{
		}

#warning remover isso do destructor da waitress e garantir exclusao da colecao

		~Waitress()
		{
			lock (__waitresses)
				__waitresses.Remove(_boss.Info.Mac);
		}

		private WaitressStatusEnum _status { get; set; }

		public CustomerOrderResponse HandleCustomerOrder(CustomerOrderRequest request)
		{
#warning trasnformar minimos em configs
			if (!_boss.Info.Enabled)
				return _custResponseFac.CurrentlyDisabled();

			if (!_boss.AllRecipesNames.Any(r => r == request.RecipeName))
				return _custResponseFac.RecipeNotAvailable();

			lock (_queue)
			{
				if (_queue.Count >= _queueCapacity)
					return _custResponseFac.FullQueue();

				var newOrder = new Order()
				{
					CustomerEmail = request.CustomerEmail,
					RecipeName = request.RecipeName,
					Reference = (uint)new Random((int)DateTime.Now.ToBinary()).Next(),
					Status = OrderStatusEnum.InQueue,
					CreationTime = DateTime.UtcNow
				};

				_queue.Enqueue(newOrder);
				var positionInQueue = _queue.Count;

				if (_status == WaitressStatusEnum.NoOrder)
					_status = WaitressStatusEnum.OrdersWaiting;

				return _custResponseFac.OrderReceived(positionInQueue, request.CustomerEmail);
			}
		}

		internal bool ThereIsOrder()
			=> _queue.Count != 0 && (int)(DateTime.UtcNow - _lastFinishedOrderTime).TotalSeconds <= _minimumSecondsBetweenOrders;

#warning add verificacao de order ref....
#warning transformar em tasks....

		internal Order GetOrder()
		{
			lock (_queue)
			{
				if (_status == WaitressStatusEnum.OrderWasTaken)
					return _orderDequeued;

				if (_status != WaitressStatusEnum.OrdersWaiting)
					return null;

				_orderDequeued = _queue.Dequeue();
				_orderDequeued.Status = OrderStatusEnum.Taken;
				_orderDequeued.TakenTime = DateTime.UtcNow;
#warning mandar email avisando que vai comecar
				_status = WaitressStatusEnum.OrderWasTaken;
				return _orderDequeued;
			}
		}

		internal bool NotifyThatOrderIsBeingProcessed()
		{
			if (_status != WaitressStatusEnum.OrderWasTaken)
				return false;

			_orderDequeued.Status = OrderStatusEnum.BeingProcessed;
			_orderDequeued.StartedTime = DateTime.UtcNow;
			_status = WaitressStatusEnum.Processing;
			return true;
		}

		internal bool NotifyThatOrderIsReady()
		{
			if (_status != WaitressStatusEnum.Processing)
				return false;

			_orderDequeued.Status = OrderStatusEnum.Ready;
			_orderDequeued.ReadyTime = DateTime.UtcNow;
			_status = _queue.Count > 0 ? WaitressStatusEnum.OrdersWaiting : WaitressStatusEnum.NoOrder;

#warning fazer emails
			//_emailSender.SendMail(_orderDequeued.CustomerEmail, TemplateEmailEnum.OrderReady);

#warning jogar no log antes de se livrar do pedido...
			_orderDequeued = null;
			return true;
		}
	}
}