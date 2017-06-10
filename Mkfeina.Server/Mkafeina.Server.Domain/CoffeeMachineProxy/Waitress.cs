using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Server.Domain.CustomerApi;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
			waitress._lastFinishedOrderTime = DateTime.UtcNow.Subtract(new TimeSpan(0,1,0));
			lock (__waitresses)
				__waitresses.Add(cmProxy.Info.Mac, waitress);

			return waitress;
		}

		#endregion Static Stuff

		#region Internal Stuff

		private CustomerResponseFactory _custResponseFac;
		private CMProxy _boss;
		private Queue<Order> _queue;
		private Order _orderUnderProcessing;
		private int _queueCapacity;
		private int _minimumSecondsBetweenOrders;
		private EmailSender _emailSender;
		private DateTime _lastFinishedOrderTime;

		#endregion Internal Stuff

		private Waitress()
		{
		}

		~Waitress()
		{
#warning remover isso do destructor da waitress e garantir exclusao da colecao
			lock (__waitresses)
				__waitresses.Remove(_boss.Info.Mac);
		}

		private WaitressStatusEnum _status;

		public CustomerOrderResponse HandleCustomerOrder(CustomerOrderRequest request)
		{
#warning trasnformar minimos em configs
			if (!_boss.Info.Enabled)
				return _custResponseFac.CMCurrentlyDisabled();

			if (!_boss.Info.HasRecipe(request.RecipeName))
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

				return _custResponseFac.OrderReceived(positionInQueue, request.CustomerEmail);
			}
		}

		internal bool ThereIsOrder()
			=> _queue.Count > 0 && (int)(DateTime.UtcNow - _lastFinishedOrderTime).TotalSeconds >= _minimumSecondsBetweenOrders;

		internal Order GetOrder()
		{
			lock (_queue)
			{
				_orderUnderProcessing = _queue.Dequeue();
				_orderUnderProcessing.Status = OrderStatusEnum.Taken;
				_orderUnderProcessing.TakenTime = DateTime.UtcNow;
#warning mandar email avisando que vai comecar
				return _orderUnderProcessing;
			}
		}
		

		internal void CancelAllOrders()
			=> Task.Factory.StartNew(() =>
			{
#warning mandar email avisando que pedido foi cancelado
				if (_orderUnderProcessing != null)
				{
					_orderUnderProcessing.Status = OrderStatusEnum.Canceled;
					_orderUnderProcessing.ReadyOrCanceledTime = DateTime.UtcNow;
				}

				foreach (var order in _queue)
				{
					_orderUnderProcessing.Status = OrderStatusEnum.Canceled;
					_orderUnderProcessing.ReadyOrCanceledTime = DateTime.UtcNow;
				}
			});

		public void Notify(ProxyEventEnum action)
		{
			return;
		}
	}
}